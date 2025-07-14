using System;
using System.Collections;
using System.Linq;
using Managers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [SerializeField] private BoxPlate[] padsToOpen;
    [SerializeField, Multiline] string messageOnLocked;
    [SerializeField, Multiline] private string messageOnUnlocked;

    [SerializeField, Tooltip("Mainly for door connected to rooms")]
    private DoorEntrance doorEntrance = DoorEntrance.NotConnectedToRoom;

    [SerializeField, Range(0, 10)] private float distanceToMoveCameraIntoRoom;
    [SerializeField] private GameObject doorSpriteRenderer;
    [SerializeField] private Transform player;
    [SerializeField] private UnityEvent onDoorOpened;
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float alternateCount;

    private ShadowCaster2D _shadowCaster;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;
    private NavMeshObstacle _navMeshObstacle;
    private bool _playerInside;
    public bool _open;
    private bool _cameraHasBeenOffset = false;
    private bool _entranceIsOnXAxis;
    public bool alternate;
    public bool _openedFirst = true;

    /// <summary>
    /// Type for where the door leads to.
    /// </summary>
    private enum DoorEntrance
    {
        Up,
        Down,
        Left,
        Right,
        NotConnectedToRoom
    }

    private void Awake()
    {
        if (doorSpriteRenderer) _spriteRenderer = doorSpriteRenderer.GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _shadowCaster = GetComponent<ShadowCaster2D>();
        if (_open)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !_open)
        {
            Messages.Instance.DisplayMessage("This door is locked. Tip: Locate the pressure plate associated with it", 1);
        }
    }

    public void CheckBoxPads()
    {
        bool allPlatesOccupied = padsToOpen.All(plate => plate.IsPlateOccupied());

        if (alternate)
        {
            var plates = padsToOpen.Where(p => p.IsPlateOccupied());
            var platesList = plates.ToList();
            if (platesList.Count >= alternateCount)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
        else
        {
            if (allPlatesOccupied)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            var cameraComposer = other.GetComponentInChildren<CinemachinePositionComposer>();
            var cinemachineCamera = other.GetComponentInChildren<CinemachineCamera>();
            var lensSettings = cinemachineCamera.Lens;
            switch (doorEntrance)
            {
                case DoorEntrance.Up:
                    _playerInside = direction.y > 0;
                    OffsetCamera(_playerInside, ref cameraComposer, distanceToMoveCameraIntoRoom, false, ref cinemachineCamera, ref lensSettings);
                    
                    break;
                case DoorEntrance.Left:
                    _playerInside = direction.x < 0;
                    OffsetCamera(_playerInside, ref cameraComposer, distanceToMoveCameraIntoRoom, true, ref cinemachineCamera, ref lensSettings);
                    break;
                case DoorEntrance.Down:
                    _playerInside = direction.y < 0;
                    OffsetCamera(_playerInside, ref cameraComposer, -distanceToMoveCameraIntoRoom, false, ref cinemachineCamera, ref lensSettings);
                    break;
                case DoorEntrance.Right:
                    _playerInside = direction.x > 0;
                    OffsetCamera(_playerInside, ref cameraComposer, -distanceToMoveCameraIntoRoom, true, ref cinemachineCamera, ref lensSettings);
                    break;
            }
            

            

        }
    }

    private void OffsetCamera(bool playerInside, ref CinemachinePositionComposer cameraComposer, float offsetAmount,
        bool entranceOnXAxis, ref CinemachineCamera cinemachineCamera, ref LensSettings lens)
    {

        if (entranceOnXAxis)
        {
            switch (playerInside)
            {
                case true when !_cameraHasBeenOffset:
                    cameraComposer.Composition.ScreenPosition.x = offsetAmount;
                    _cameraHasBeenOffset = true;
                    lens.OrthographicSize = zoomAmount;
                    break;
                case false:
                    _cameraHasBeenOffset = false;
                    lens.OrthographicSize = 1.5f;
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
                    lens.OrthographicSize = zoomAmount;
                    _cameraHasBeenOffset = true;
                    break;
                case false:
                    _cameraHasBeenOffset = false;
                    cameraComposer.Composition.ScreenPosition.y = 0;
                    lens.OrthographicSize = 1.5f;
                    break;
            }
        }
        cinemachineCamera.Lens = lens;

    }

    public void OpenDoor()
    {
        if (_spriteRenderer) _spriteRenderer.enabled = false;
        _collider2D.isTrigger = true;
        _navMeshObstacle.enabled = false;
        _shadowCaster.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
        _open = true;
        Messages.Instance.DisplayMessage("Door Opened", 1);
        if (!_openedFirst) StartCoroutine(Check());
        //MessageManager.Instance.ShowMessage(messageOnUnlocked);
    }

    private IEnumerator Check()
    {
        var target = new CameraTarget();
        if (player)
        {
            target.TrackingTarget = this.transform;
            var playerCamera = player.GetComponentInChildren<CinemachineCamera>();
            var playerController = player.GetComponent<PlayerController>();
            var playerDrag = player.GetComponent<PlayerDrag>();
            playerController.UnsubscribeInputs();
            playerCamera.Target = target;
            yield return new WaitForSeconds(1.5f);
            target.TrackingTarget = player;
            playerCamera.Target = target;
            playerController.SubscribeInputs();
            playerDrag.DetachFromObject();
            yield return null;
        }

    }
    
        public void CloseDoor()
    {
        if (_spriteRenderer) _spriteRenderer.enabled = true;
        _collider2D.isTrigger = false;
        _navMeshObstacle.enabled = true;
        _shadowCaster.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Wall");

        _open = false;

    }
}

