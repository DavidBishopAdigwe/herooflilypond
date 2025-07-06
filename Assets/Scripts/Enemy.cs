using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PlayerScripts;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.AI;

[SuppressMessage("ReSharper", "Unity.UnknownTag")]
[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
[SuppressMessage("Resharper", "Unity.PerformanceAnalysis")]

public class Enemy : MonoBehaviour
{
    
    private NavMeshAgent _agent; 
    [SerializeField] private GameObject lightSource;
    [SerializeField] private int numberOfRays;
    [SerializeField] private float coneAngle;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float rayLength;
    [SerializeField] private float lightRotationSpeed = 5f;
    [SerializeField] private float speed;
    [SerializeField] private float maxChaseSpeed;
    [SerializeField] private float speedIncreasePerSecondWhileChasing;
    private SpriteRenderer _spriteRenderer;
    private float _currentSpeed;
    private Transform _player;
    private MovementArea _movementArea;
    private int _numberOfMovementPoints;
    private int _index;
    private Vector3 _currentPoint;
    private Coroutine _currentCoroutine;
    private bool _chasingPlayer;
    private PlayerHide _playerHide;
    private bool _playerInRange;
    private bool _isSwitchingState;
    private readonly List<Transform> _movementPoints = new();
    
    private PlayerHide PlayerHideSetter 
    {
        set
        {
            if (!_playerHide) _playerHide = value;
        }
    }
    

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentSpeed = speed;
    }

    public void Setup(MovementArea area)
    {
        Transform areaTransform = area.transform;
        
        foreach (Transform child in areaTransform)
        {
            _movementPoints.Add(child);
        }
        
        _numberOfMovementPoints = _movementPoints.Count;
        Debug.Log(_movementPoints[0].name);
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _currentSpeed;
        StartingNavMeshSettings();
        _currentCoroutine =  StartCoroutine(MoveAbout());
    }
    
    private void StartingNavMeshSettings() // Navmesh settings to ensure it works in 2D
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
   

    private void ChoosePoint()
    {
        _currentPoint = _movementPoints[_index].position;
        _index++;
        if (_index == _numberOfMovementPoints) _index = 0; 
    }
    
    
   private void SwitchCoroutine(IEnumerator newRoutine)
    {
        if (_currentCoroutine != null)
        {
            if (newRoutine.GetType() == _currentCoroutine.GetType()) return;
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(newRoutine);
    }

   

    private IEnumerator MoveAbout()
    {
        _chasingPlayer = false;
        ChoosePoint();
        while (!_chasingPlayer)
        {
            if (Vector3.Distance(_agent.transform.position, _currentPoint) < 0.1f) 
            {
                ChoosePoint();
            }
            _agent.SetDestination(_currentPoint);
            DetectPlayer();
            yield return null;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void DetectPlayer()
    {

        if (!_chasingPlayer || !_playerHide.IsHidingEffectInProgress()) return;

        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = _agent.velocity.normalized; 

        for (int i = 0; i < numberOfRays; i++)
        {
            float raySeparation        = coneAngle / (numberOfRays - 1);
            float overallOffset        = -coneAngle / 2 + i * raySeparation; 
            Vector2 spreadDirection = Quaternion.Euler(0, 0, overallOffset) * rayDirection;

            RaycastHit2D hit           = Physics2D.Raycast(rayOrigin, spreadDirection, rayLength, playerLayer);
            
            Debug.DrawRay(rayOrigin, spreadDirection * rayLength, Color.red); // TODO: Take ts out
            
            Transform playerTransform;

            if (_playerHide.IsHidingEffectInProgress() && hit && hit.collider.CompareTag("Player"))
            {
                _playerInRange = true;
                return;
            }
            _playerInRange = false;
            switch (hit.collider)
            {
                case not null when hit.collider.CompareTag("Player") && 
                                   NavMesh.SamplePosition(hit.collider.transform.position, out NavMeshHit hitPlayerMesh, 1f, NavMesh.AllAreas):
                    
                    playerTransform = hit.collider.transform;
                    StartChase(ref playerTransform);
                    break;
                case not null when hit.collider.CompareTag("PlayerLight") && 
                                   NavMesh.SamplePosition(hit.collider.transform.parent.position, out NavMeshHit hitLight, 1f, NavMesh.AllAreas):
                    
                     playerTransform = hit.collider.transform.parent;
                     StartChase(ref playerTransform);
                    break;
            }
        }
    }

    private void StartChase( ref Transform player)
    {
        _player = player;
        PlayerHideSetter = _player.GetComponent<PlayerHide>();
        SwitchCoroutine(ChasePlayer());
       // StartCoroutine(IncreaseSpeed());
        

    }


    private IEnumerator ChasePlayer()
    {
        _chasingPlayer = true;
        while (_chasingPlayer)
        {
            DetectPlayer(); 
            Vector2 direction = _agent.velocity.normalized;
            lightSource.transform.up = direction;
            if (NavMesh.SamplePosition(_player.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
            else
            {
                SwitchCoroutine(MoveAbout());
                yield break;
            }
            
            if (_playerHide.IsHidingEffectInProgress() && _playerInRange)
            {
                _playerHide.StopPlayerHiding();
            }
            
            UpdateFacingDirection(direction);

            yield return null;
        }
    }
    
    
    

    private void UpdateFacingDirection(Vector2 direction)
    {
        _spriteRenderer.flipX = !(direction.x > 0);
    }
    

    private IEnumerator IncreaseSpeed()
    {
        while (_chasingPlayer && _agent.speed < maxChaseSpeed)
        {
            _currentSpeed += speedIncreasePerSecondWhileChasing * Time.deltaTime;
            _agent.speed = Mathf.Min(_currentSpeed, maxChaseSpeed);
            yield return null;
        } 
    }
    
}
