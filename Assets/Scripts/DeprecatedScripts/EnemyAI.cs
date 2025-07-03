using System.Collections;
using System.Diagnostics.CodeAnalysis;
using PlayerScripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace DeprecatedScripts
{
    [SuppressMessage("ReSharper", "Unity.UnknownTag")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    public class EnemyAI : MonoBehaviour
    { 
        [SerializeField] private float moveSpeed;
        [SerializeField] private LayerMask playerLayer; 
        [SerializeField] private float rayLength;
        [SerializeField] private float speed;
        [SerializeField] private int numberOfRays; 
        [SerializeField] private float coneAngle; 
        [SerializeField] private GameObject lightSource;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private string layer1;
        [SerializeField] private string layer2;
        [SerializeField] private string defaultLayer; 
        [SerializeField] private Light2D spotLight;
        [SerializeField] private float lerpSpeed;
        [SerializeField] private int damage;
        [SerializeField] private Collider2D detectionCollider;
        [SerializeField] private string currentLayer;
        [SerializeField] private float secondsTillEnemySpeedCatchesUp;
        [SerializeField] private float maxChaseSpeed;
        [SerializeField] private float speedIncreasePerSecond;
    
        private Transform[] _movementPoints;
        private Transform _player;
        private float _currentChaseSpeed;
        private int _randomIndex;
        private bool _playerInRange;
        private Vector2 _previousDirection;
        private bool _chasingPlayer;
        private Coroutine _currentCoroutine;
        private Transform _currentPoint;
        private Transform _previousPoint;
        private float _distanceToStopChase = 0.2f;
        private NavMeshAgent _agent;
        private int _iterations;
        private const int MaxIterations = 1000;
        private bool _onDisconnectedPoint;
        private Transform _storagePoint;
        public int currentAgentId = -1;
        private PlayerHide _playerHide;
        private const int AgentLayerID = 11;
        private const int Layer1ID = 20;
        private const int Layer2ID = 21;
        private bool _isSwitchingState;
        private float _stateSwitchCooldown = 0.5f;
        private bool panningPositive;
        private bool panningNegative;
        private bool panning;


        private void Awake()
        {
            GetComponentsInAwake();
            _currentChaseSpeed = speed;
            _agent.speed = _currentChaseSpeed;
        }

        private void Start()
        {
            StartingNavMeshSettings();
            AssignParametersInStart();
            _randomIndex = Random.Range(0, _movementPoints.Length);
            AddEnemyToList();
            _currentCoroutine = StartCoroutine(Wander());
            CheckLayer(); 
            GetPositionsInStart();
        }

        private void CheckLayer() // Map specific layer logic, final map may not need this
        {
            switch (gameObject.layer)
            {
                case Layer1ID:
                    Physics2D.IgnoreLayerCollision(AgentLayerID, Layer1ID, false);
                    Physics2D.IgnoreLayerCollision(AgentLayerID, Layer2ID, true);
                    break;
                case Layer2ID:
                    Physics2D.IgnoreLayerCollision(AgentLayerID, Layer2ID, false);
                    Physics2D.IgnoreLayerCollision(AgentLayerID, Layer1ID, true);
                    break;
                
            }
            int sortingLayerId = SortingLayer.NameToID(LayerMask.LayerToName(gameObject.layer));
        
            // Part of unity package cache, reverts to private, looking for new solution
        
            /*  _light.m_ApplyToSortingLayers = new int[] { sortingLayerId };
        spotLight.m_ApplyToSortingLayers = new int[] { sortingLayerId }; */  
        }

        private void AssignParametersInStart()
        {
            /*_movementPoints = AssignmentManager.Instance.GetAllMovementPoints();
         _playerHide = AssignmentManager.Instance.GetPlayerHide();*/
        }
    
        private void GetComponentsInAwake()
        {
            _agent = GetComponent<NavMeshAgent>();
            sprite = GetComponent<SpriteRenderer>();
        }

        private void StartingNavMeshSettings() // Navmesh settings to ensure it works in 2D
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }


    
        private IEnumerator Wander()
        {
            while (!_chasingPlayer) 
            {
            
                if (!_agent.isOnNavMesh)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
                    {
                        _agent.Warp(hit.position);
                    }
                    else
                    {
                        yield return null;
                        continue;
                    }
                }
            
                if (_currentPoint == null || Vector2.Distance(transform.position, _currentPoint.position) <= _distanceToStopChase ) 
                {
                    Chooser();
                }
            
                if (_currentPoint != null) {_agent.SetDestination(_currentPoint.position);}
            
                Vector2 direction = _agent.velocity.normalized;
                UpdateFacingDirection(direction);
                lightSource.transform.up = Vector2.Lerp(lightSource.transform.up, direction, lerpSpeed * Time.deltaTime);
                DetectPlayer();
                yield return null;
            }
        }
    



        private void Chooser()
        {
            int attempts = 0;

            while (attempts < MaxIterations)
            {
                _randomIndex = Random.Range(0, _movementPoints.Length);
                Transform chosenPoint = _movementPoints[_randomIndex];

                if (chosenPoint == _previousPoint || chosenPoint.TryGetComponent(out WalkingPoints walkPoint) && walkPoint.GetCurrentLayer() != sprite.sortingLayerName)
                {
                    attempts++;
                    continue;
                }

                NavMeshPath path = new NavMeshPath();
                if (_agent.isOnNavMesh && _agent.CalculatePath(chosenPoint.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    _previousPoint = _currentPoint;
                    _currentPoint = chosenPoint;
                    break;
                }
                attempts++;
            }

            if (attempts >= MaxIterations)
            {
                print("failed to find a path");
                _currentPoint = null;
            }
       
        }
  
  

        private void DetectPlayer()
        {
        
            if (_isSwitchingState || _playerHide.IsHidden()) return; 
            Vector2 rayOrigin = transform.position;
            Vector2 rayDirection = _agent.velocity.normalized; 

            for (int i = 0; i < numberOfRays; i++)
            {
                var raySeparation = coneAngle / (numberOfRays - 1);
                var overallOffset = -coneAngle / 2 + i * raySeparation; 
                Vector2 spreadDirection = Quaternion.Euler(0, 0, overallOffset) * rayDirection;

                RaycastHit2D hit = 
                    Physics2D.Raycast(rayOrigin, spreadDirection, rayLength, playerLayer);
                Debug.DrawRay(rayOrigin, spreadDirection * rayLength, Color.red);

                if (hit.collider )
                {
                    if (hit.collider.TryGetComponent(out SpriteRenderer playerRenderer) &&
                        playerRenderer.sortingLayerName == sprite.sortingLayerName)
                    {
                        _player = hit.collider.transform;
                        _chasingPlayer = true;
                        SwitchCoroutine(ChasePlayer());
                    }
                    else if (hit.collider.CompareTag("PlayerLight") && hit.collider.transform.parent.TryGetComponent(out SpriteRenderer spriteRenderer) &&
                             spriteRenderer.sortingLayerName == sprite.sortingLayerName)
                    {
                        _player = hit.collider.transform.parent;
                        _chasingPlayer = true;
                        SwitchCoroutine(ChasePlayer());
                        StartCoroutine(IncreaseSpeed());
                    }
               
                    break;
                }
                _chasingPlayer = false;
            
            }
        } 
    
    

        private void StopChasingPlayer()
        {
            _chasingPlayer = false;
            _agent.speed = speed;
            SwitchCoroutine(Wander());
        }

        private void UpdateFacingDirection(Vector2 direction)
        {
            if (direction.x < 0) sprite.flipX = true;
            else if (direction.x > 0) sprite.flipX = false;
        }


        private void DetectPlayerWhileHiding()
        {
            if (!_playerHide.HidingEffectInProgress()) return;
        
            Vector2 rayOrigin = transform.position;
            Vector2 rayDirection = _agent.velocity.normalized; 

            for (int i = 0; i < numberOfRays; i++)
            {
                var raySeparation = coneAngle / (numberOfRays - 1);
                var overallOffset = -coneAngle / 2 + i * raySeparation; 
                Vector2 spreadDirection = Quaternion.Euler(0, 0, overallOffset) * rayDirection;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, spreadDirection, rayLength, playerLayer);
                Debug.DrawRay(rayOrigin, spreadDirection * rayLength, Color.red);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    _playerInRange = true;
                    break;
                }
                _playerInRange = false;

            }
        }
        private IEnumerator ChasePlayer()
        {
            while (_chasingPlayer)
            {
                if (!_agent.isOnNavMesh)
                {
                    StopChasingPlayer();
                    yield break;
                }
            
            
                DetectPlayerWhileHiding();
                Vector2 direction = _agent.velocity.normalized;
                lightSource.transform.up =
                    Vector2.Lerp(lightSource.transform.up,direction, lerpSpeed * Time.deltaTime );

                if (NavMesh.SamplePosition(_player.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                }
                else
                { 
                    yield return StartCoroutine(StopChasingPlayerCooldown());
                    yield break;
                }
            
                if (_playerHide.HidingEffectInProgress() && _playerInRange)
                {
                    _playerHide.StopPlayerHiding();
                }
            
                if (_playerHide.IsHidden())
                {
                    StopChasingPlayer();
                }
                UpdateFacingDirection(direction);

                yield return null;
            }
        
        }

        private IEnumerator IncreaseSpeed()
        {
            while (_chasingPlayer && _agent.speed < maxChaseSpeed)
            {
                _currentChaseSpeed += speedIncreasePerSecond * Time.deltaTime;
                _agent.speed = Mathf.Min(_currentChaseSpeed, maxChaseSpeed);
                yield return null;
            } 
        }
    

        private void OnTriggerEnter2D(Collider2D other)
        {

            if (other.TryGetComponent(out SpriteRenderer spriteRenderer) &&
                spriteRenderer.sortingLayerName == sprite.sortingLayerName && other.TryGetComponent(out Health playerHealth))
            {
                _playerHide.ResetHideEffect();
                playerHealth.TakeDamage(damage);
            } 
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

        private IEnumerator StopChasingPlayerCooldown()
        {
            _isSwitchingState = true;
            StopChasingPlayer();
            yield return new WaitForSeconds(_stateSwitchCooldown);
            _isSwitchingState = false;
        }
    
        private void AddEnemyToList()
        {
            EnemyPositionManager.Instance.AddEnemy(this);
        }

        private void GetPositionsInStart()
        {
            EnemyPositionManager.Instance.GetEnemyPositions();
        }

        public IEnumerator ResetEnemies()
        {
            _chasingPlayer = false;
            yield return new WaitForSecondsRealtime(1f);
            SwitchCoroutine(Wander());
        }

        public void AssignMovementArea(MovementArea area)
        {
        
        }

        /* = private IEnumerator PanWhilePlayerIsHidden()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        float cosXValue = Mathf.Cos(direction.x);
        float sinXValue = Mathf.Sin(direction.y);
        float baseNoCos = cosXValue;
        float baseNoSin = sinXValue;
        float angle = (float)Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        panningPositive = true;
        while (panning)
        {
            if (baseNoCos >= cosXValue + cosXValue / 2 && baseNoSin >= sinXValue + (sinXValue / 2))
            {
                panningPositive = false;
            }
            else if (baseNoCos <= cosXValue - cosXValue / 2 && baseNoSin <= sinXValue - (sinXValue / 2))
            {
                panning = false;
                SwitchCoroutine(Wander());
            }
            if (panningPositive)
            {
                baseNoCos = Mathf.Clamp(Mathf.Acos(baseNoCos + (cosXValue / 2 * Time.deltaTime)),0 , cosXValue + cosXValue / 2) ;
                baseNoSin = Mathf.Clamp(Mathf.Asin(baseNoSin + (sinXValue / 2 * Time.deltaTime)), 0, sinXValue + (sinXValue / 2)) ;
            }
            else
            {
                baseNoCos = Mathf.Clamp(Mathf.Acos(baseNoCos - (cosXValue / 2 * Time.deltaTime)),0, cosXValue - cosXValue / 2) ;
                baseNoSin = Mathf.Clamp(Mathf.Asin(baseNoSin - (sinXValue / 2 * Time.deltaTime)), 0, sinXValue - (sinXValue / 2)) ;
            }
            
            Vector2 newDirection = new Vector2(baseNoCos, baseNoSin).normalized;
           lightSource.transform.up = Vector2.Lerp(lightSource.transform.up, newDirection, lerpSpeed * Time.deltaTime );
           yield return null;
        }
    } */

    }
}
