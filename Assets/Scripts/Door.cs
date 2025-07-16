using System;
using System.Collections;
using System.Linq;
using Enums;
using Managers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [SerializeField] private BoxPlate[] platesToOpen;
    [SerializeField, Multiline] string messageOnLocked;
    [SerializeField, Multiline] private string messageOnUnlocked;
    [SerializeField] private SeperatedDoorObject[] doorParts;

    [SerializeField, Tooltip("Mainly for door connected to rooms")]
    private DoorEntrance doorEntrance = DoorEntrance.NotConnectedToRoom;

    [SerializeField, Range(0, 1)] private float distanceToMoveCameraIntoRoom;
    [SerializeField] private float zoomAmount = 1.5f;
    
    [SerializeField, Tooltip("Some doors may have different plates but need a certain amount to unlock it. This and the {unlockWithLess} boolean are applicable here")] 
    private float platesToUnlockCount;

    [SerializeField] private bool unlockWithLess;
    [SerializeField] private bool panPlayerCamera;
    [SerializeField] private float panTime = 1.5f;
    [SerializeField] private bool startOpen;
    
    private enum DoorState
    {
        Opened, Closed
    }
    
    
    private ShadowCaster2D _shadowCaster;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private NavMeshObstacle _navMeshObstacle;
    private bool _playerInside;
    public bool _open;
    public bool _cameraHasBeenOffset = false;
    private bool _entranceIsOnXAxis;
    public bool alternate;
    private const float BaseLensSize = 1.5f;
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
        _collider = GetComponent<Collider2D>();
        _shadowCaster = GetComponent<ShadowCaster2D>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
    }

    private void Start()
    {
        if (startOpen)
        {
            StartOpen();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MessageMaster.Instance.ShowMessage("Door locked.");
        }
    }

    public void CheckBoxPads()
    {
        if (unlockWithLess)
        {
            int activePlates = platesToOpen.Count(plate => plate.IsPlateOccupied());
        
            if (activePlates >= platesToUnlockCount && !_open)
            {
                OpenDoor();
            }
            else if (activePlates < platesToUnlockCount && _open)
            {
                CloseDoor();
            }
        }
        else
        {
            bool allPlatesOccupied = platesToOpen.All(plate => plate.IsPlateOccupied());

            if (allPlatesOccupied && !_open)
            {
                OpenDoor();
            }
            else if (!allPlatesOccupied && _open)
            {
                CloseDoor();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerPassed");
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
        Debug.Log("OffsetCamera");
        if (playerInside) {Debug.Log("Inside");}
        if (entranceOnXAxis)
        {
            switch (playerInside)
            {
                case true when !_cameraHasBeenOffset:
                    cameraComposer.Composition.ScreenPosition.x = offsetAmount;
                    _cameraHasBeenOffset = true;
                    lens.OrthographicSize = zoomAmount;
                    Debug.Log("OffsetCameraPlayerInsideX");
                    break;
                case false:
                    _cameraHasBeenOffset = false;
                    lens.OrthographicSize = BaseLensSize;
                    cameraComposer.Composition.ScreenPosition.x = 0;
                    Debug.Log("ResetToBase");
                    
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
                    Debug.Log("OffsetCameraPlayerInsideY");
                    break;
                case false:
                    _cameraHasBeenOffset = false;
                    cameraComposer.Composition.ScreenPosition.y = 0;
                    lens.OrthographicSize = BaseLensSize;
                    Debug.Log("ResetToBase");

                    break;
            }
        }
        cinemachineCamera.Lens = lens;

    }

    public void OpenDoor()
    {
        foreach (var part in doorParts)
        {
            part.DoorOpened();
        }
        _open = true;
        _collider.isTrigger = true;
        if (_shadowCaster) _shadowCaster.enabled = false;
        _navMeshObstacle.enabled = false;
       MessageMaster.Instance.ShowMessage("Door Opened", MessageType.Success);
       DoorSystems.Instance.OnDoorOpened();
       if (panPlayerCamera) StartCoroutine(DoorSystems.Instance.PanToDoor(this.transform, panTime));
    }

    private void StartOpen()
    {
        foreach (var part in doorParts)
        {
            part.DoorOpened();
        }

        _collider.isTrigger = true; 
        if (_shadowCaster) _shadowCaster.enabled = false; // Front facing doors lack a shadow caster atm, 
        _navMeshObstacle.enabled = false;
        _open = true;
    }


    
        public void CloseDoor()
    {
        foreach (var part in doorParts)
        {
            part.DoorClosed();
        }
        _collider.isTrigger = false; 
        if (_shadowCaster) _shadowCaster.enabled = true;
        _navMeshObstacle.enabled = true;
        MessageMaster.Instance.ShowMessage("Door Closed", MessageType.Error);
        gameObject.layer = LayerMask.NameToLayer("Wall");
        _open = false;

    }
}

