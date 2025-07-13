using System;
using System.Linq;
using Managers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Door: MonoBehaviour
    {
        [SerializeField] private BoxPlate[] padsToOpen;
        [SerializeField, Multiline]  string messageOnLocked;
        [SerializeField, Multiline] private string messageOnUnlocked; 
        [SerializeField, Tooltip("Mainly for door connected to rooms")] private DoorEntrance doorEntrance = DoorEntrance.NotConnectedToRoom;
        [SerializeField, Range(0, 10)] private float distanceToMoveCameraIntoRoom;

        private ShadowCaster2D _shadowCaster;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;
        private NavMeshObstacle _navMeshObstacle;
        private bool _playerInside;
        public bool _open;
        private bool _cameraHasBeenOffset = false;
        private bool _entranceIsOnXAxis;
        /// <summary>
        /// Type for where the door leads to.
        /// </summary>
        private enum DoorEntrance
        {
            Up, Down, Left, Right, NotConnectedToRoom
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _shadowCaster = GetComponent<ShadowCaster2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && !_open )
            {
                MessageManager.Instance.ShowMessage(messageOnLocked);
            }
        }

        public void CheckBoxPads()
        {
            bool allPlatesOccupied = padsToOpen.All(plate => plate.IsPlateOccupied());

            if (allPlatesOccupied)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
            
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                var cameraComposer = other.GetComponentInChildren<CinemachinePositionComposer>();
                switch (doorEntrance)
                {
                    case DoorEntrance.Up:
                        _playerInside = direction.y > 0;
                        OffsetCamera(_playerInside, ref cameraComposer, distanceToMoveCameraIntoRoom, false);
                        break;
                    case DoorEntrance.Left:
                        _playerInside = direction.x < 0;
                        OffsetCamera(_playerInside, ref cameraComposer, distanceToMoveCameraIntoRoom, true);
                        break;
                    case DoorEntrance.Down:
                        _playerInside = direction.y < 0;
                        OffsetCamera(_playerInside, ref cameraComposer, -distanceToMoveCameraIntoRoom, false);
                        break;
                    case DoorEntrance.Right:
                        _playerInside = direction.x > 0;
                        OffsetCamera(_playerInside, ref cameraComposer, -distanceToMoveCameraIntoRoom, true);
                        break;
                }
                
            }
        }

        private void OffsetCamera( bool playerInside, ref CinemachinePositionComposer cameraComposer,  float offsetAmount, bool entranceOnXAxis)
        {
            if (entranceOnXAxis)
            {
                switch (playerInside)
                {
                    case true when !_cameraHasBeenOffset:
                        cameraComposer.Composition.ScreenPosition.x = offsetAmount;
                        _cameraHasBeenOffset = true;
                        break;
                    case false:
                        _cameraHasBeenOffset = false;
                        cameraComposer.Composition.ScreenPosition.x = 0;
                        break;
                }
            }
            else
            {
                switch (playerInside)
                {
                    case true when !_cameraHasBeenOffset:
                        cameraComposer.Composition.ScreenPosition.y = offsetAmount;
                        _cameraHasBeenOffset = true;
                        break;
                    case false:
                        _cameraHasBeenOffset = false;
                        cameraComposer.Composition.ScreenPosition.y = 0;
                        break;
                }
            }
        }

        public void OpenDoor()
        {
            _spriteRenderer.enabled = false;
            _collider2D.isTrigger = true;
            _navMeshObstacle.enabled = false;
            _shadowCaster.enabled = false;
            _open = true;
            //MessageManager.Instance.ShowMessage(messageOnUnlocked);
        }

        public void CloseDoor()
        {
            _spriteRenderer.enabled = true;
            _collider2D.enabled = true;
            _navMeshObstacle.enabled = true;
            _open = false;
        }

        private void FixedUpdate()
        {
            if (_open)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }

