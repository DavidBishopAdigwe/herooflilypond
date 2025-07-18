
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PlayerScripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[SuppressMessage("ReSharper", "Unity.UnknownTag")]
[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
[SuppressMessage("Resharper", "Unity.PerformanceAnalysis")]

public class Enemy : MonoBehaviour
{
    
    private NavMeshAgent _agent; 
    [SerializeField] private GameObject lightSource;
    [SerializeField] private int numberOfRays;
    [SerializeField] private float coneAngle;
    [SerializeField] private LayerMask visionLayers;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayLength;
    [SerializeField] private float lightRotationSpeed;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float maxChaseSpeed; 
    [SerializeField] private float speedIncreasePerSecondWhileChasing;
    [SerializeField] private float secondsToStopIfPlayerIsNotSeen;
    [SerializeField] private int damage;

    private float _currentSpeed;
    private AudioSource _audioSource;
    private Transform _player;
    private MovementArea _movementArea;
    private int _numberOfMovementPoints;
    private int _index;
    private Coroutine _stateRoutine;
    private PlayerHide _playerHide;
    private bool _isSwitchingState;
    private EnemyState _enemyState;
    private Transform _currentPoint;
    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[25]; // doubt it's possible to even hit 10 but just incase
    private readonly List<Transform> _movementPoints = new();
    private Light2D[] _visionLights;
    private bool _canDetect = true;
    private Coroutine _cooldownRoutine;
    private float _lastSeenTime;
    private float _pathCheckTimer;
    private float _lastRayTime;
    private int _iterations = 0;

    private const int MaxIterations = 1000;
    private enum EnemyState
    {
        Wandering, Chasing
    }
    
    

    private void Awake()
    {
        _currentSpeed = normalSpeed;
        _visionLights = GetComponentsInChildren<Light2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ToggleLightSource(false);
    }

    public void Setup(MovementArea area)
    {
        Transform areaTransform = area.transform;
        
        foreach (Transform child in areaTransform)
        {
            _movementPoints.Add(child);
        }
        
        _numberOfMovementPoints = _movementPoints.Count;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _currentSpeed;
        StartingNavMeshSettings();
        _audioSource.Play();
        _stateRoutine =  StartCoroutine(MoveAbout());
    }
    
    private void StartingNavMeshSettings() // Navmesh settings to ensure it works in 2D
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
   

 
    
   private void SwitchCoroutine(IEnumerator newRoutine)
    {
        if (_stateRoutine != null)
        {
            if (newRoutine.GetType() == _stateRoutine.GetType()) return;
            StopCoroutine(_stateRoutine);
        }
        _stateRoutine = StartCoroutine(newRoutine);
    }


    private void ChoosePoint()
    {
        _currentPoint = _movementPoints[_index];
        _index++;
        if (_index == _numberOfMovementPoints) _index = 0; 
    }

    private IEnumerator MoveAbout()
    {
        _agent.speed = normalSpeed;
        _enemyState = EnemyState.Wandering;
        ChoosePoint();
        _agent.SetDestination(_currentPoint.position);
        var path = new NavMeshPath();

        while (_enemyState == EnemyState.Wandering)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.05f ||
                !_agent.CalculatePath(_currentPoint.position, path) && path.status != NavMeshPathStatus.PathComplete)
            {
                _iterations++;
                if (_iterations == MaxIterations) yield break;
                ChoosePoint();
                _agent.SetDestination(_currentPoint.position);
            }

            _iterations = 0;
            DetectPlayer();
            UpdateFacingDirection();
            yield return null;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void DetectPlayer()
    {
        if (!_canDetect)
        {
            return;
        }
        
        NavMeshPath path = new NavMeshPath();
        
        if (_enemyState ==  EnemyState.Chasing && !_playerHide.IsHidingInProgress()) return;
        
        SwitchCoroutine(RayCooldown(), ref _cooldownRoutine);
        Vector2 rayDirection = _agent.desiredVelocity.normalized;

        for (int i = 0; i < numberOfRays; i++)
        {

            float raySeparation = coneAngle / (numberOfRays - 1);
            float overallOffset = -coneAngle / 2 + i * raySeparation;
            Vector2 spreadDirection = Quaternion.Euler(0, 0, overallOffset) * rayDirection;

            ContactFilter2D contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = visionLayers,
                useTriggers = true
            };
            var hitCount = Physics2D.Raycast(rayOrigin.position, spreadDirection, contactFilter, _hitBuffer, rayLength);
            
            for (int j = 0; j < hitCount; j++)
            {

                var hit = _hitBuffer[j];

                if (hit.collider.CompareTag("Wall")) break;

                if (_playerHide && _playerHide.IsHidingInProgress()
                                   && hit.collider.CompareTag("Player"))
                {
                    _playerHide.StopPlayerHiding();
                    break;
                }

                if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("PlayerLight")) continue;
                
                _lastSeenTime = Time.time;

                Transform playerTransform = hit.collider.CompareTag("Player") ? hit.collider.transform : hit.collider.transform.parent;

                if (_agent.CalculatePath(playerTransform.position, path) &&
                           path.status == NavMeshPathStatus.PathComplete && 
                           _enemyState != EnemyState.Chasing)
                {
                    StartChase( playerTransform);
                }
                

                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);
            SwitchCoroutine(StopChasingPlayer());
        }
    }

    private void SetPlayerHide( Transform player)
    {
        if (!_playerHide)
        {
            _playerHide = player.GetComponent<PlayerHide>();
        }
    }
    private void StartChase( Transform player)
    {
        _player = player;
        SetPlayerHide( player);
        SwitchCoroutine(ChasePlayer()); 
        StartCoroutine(IncreaseSpeed());
    }

    private IEnumerator StopChasingPlayer()
    {
        _currentSpeed = normalSpeed;
        _agent.speed = _currentSpeed;
        gameObject.transform.position = _movementPoints[0].position;
        yield return new WaitForSeconds(0.5f);
        SwitchCoroutine(MoveAbout());
    }


    private IEnumerator ChasePlayer()
    {
        _lastSeenTime = Time.time;
        _enemyState = EnemyState.Chasing;
        while (_enemyState == EnemyState.Chasing)
        {
           DetectPlayer(); 

           if (Time.time - _lastSeenTime > secondsToStopIfPlayerIsNotSeen)
           {
               SwitchCoroutine(MoveAbout());
               yield break;
           } 
           if (NavMesh.SamplePosition(_player.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) 
           {
                _agent.SetDestination(hit.position); 
           }
           else 
           {
                SwitchCoroutine(MoveAbout());
                yield break;
           }

           if (_playerHide.IsHidingInProgress())
           {
               _canDetect = true;
               DetectPlayer();
           }

           if (_playerHide.IsHidden()) SwitchCoroutine(MoveAbout());
            
           UpdateFacingDirection();

           yield return null;
        }
    }
    
    
    

    private void UpdateFacingDirection()
    {
        Vector2 direction = _agent.desiredVelocity.normalized;
        lightSource.transform.up = Vector2.Lerp(lightSource.transform.up, direction, lightRotationSpeed * Time.deltaTime);
        var scale = transform.localScale;
        scale.x = direction.x > 0 ? 1 : -1;
        transform.localScale = scale;
    }



    private IEnumerator IncreaseSpeed()
    {
        while (_enemyState == EnemyState.Chasing && _agent.speed < maxChaseSpeed)
        {
            _currentSpeed += speedIncreasePerSecondWhileChasing * Time.deltaTime;
            _agent.speed = Mathf.Min(_currentSpeed, maxChaseSpeed);
            if (_agent.speed >= maxChaseSpeed)
            {
                _agent.speed = (maxChaseSpeed);
            }
            yield return null;
        } 
    }

    private void SwitchCoroutine(IEnumerator newRoutine, ref Coroutine routineToSwap)
    {
        if (routineToSwap != null)
        {
            StopCoroutine(routineToSwap);
            routineToSwap = null;
        }
        StartCoroutine(newRoutine);
    }


    private IEnumerator RayCooldown()
    {
        _canDetect = false;
        yield return new WaitForSeconds(0.1f);
        _canDetect = true;
    }

    public void ToggleLightSource(bool toggle)
    {
            foreach (var l in _visionLights)
            {
                l.enabled = toggle;
            }
    }

    private void OnDestroy()
    {
        _audioSource.Stop();
    }
}
