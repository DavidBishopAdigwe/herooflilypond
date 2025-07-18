using System;
using System.Collections;
using System.Linq;
using Enums;
using Singletons;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [SerializeField] private BoxPlate[] platesToOpen;
    [SerializeField] private SeperatedDoorObject[] doorParts;

    [SerializeField, Tooltip("Mainly for door connected to rooms")]
    private DoorEntrance doorEntrance = DoorEntrance.NotConnectedToRoom;

    [SerializeField, Range(0, 1)] private float distanceToMoveCameraIntoRoom;
    [SerializeField] private float zoomAmount = 1.5f;
    
    [SerializeField] private bool panPlayerCamera;
    [SerializeField] private float panTime = 1.5f;
    [SerializeField] private bool startOpen;
    
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
    private ShadowCaster2D _shadowCaster;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private NavMeshObstacle _navMeshObstacle;
    private AudioSource _audioSource;
    private bool _playerInside;
    private bool _doorOpen;
    private bool _cameraHasBeenOffset = false;
    private bool _entranceIsOnXAxis;
    private const float BaseLensSize = 1.5f;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _shadowCaster = GetComponent<ShadowCaster2D>();
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _audioSource = GetComponent<AudioSource>();
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
            MessageManager.Instance.ShowMessage("Door locked.", MessageType.Default);
        }
    }

    public void CheckBoxPads()
    {
        int activePlates = platesToOpen.Count(plate => plate.IsPlateOccupied());
        bool allPlatesOccupied = platesToOpen.All(plate => plate.IsPlateOccupied());
        
        if (allPlatesOccupied && !_doorOpen)
        {
            OpenDoor();
        }
        else if (!allPlatesOccupied && _doorOpen)
        {
            CloseDoor();
        }
        if (platesToOpen.Count() > 1 && activePlates < platesToOpen.Count())
        {
            MessageManager.Instance.ShowMessage($"{activePlates}/{platesToOpen.Length} plates remain");
            StartCoroutine(PanSystem.Instance.PanToDoor(transform, panTime)); 
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
                    lens.OrthographicSize = BaseLensSize;
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
                    lens.OrthographicSize = BaseLensSize;

                    break;
            }
        }
        cinemachineCamera.Lens = lens;

    }

    public void OpenDoor()
    {
        StartOpen();
       MessageManager.Instance.ShowMessage("Door Opened", MessageType.Success);
       PanSystem.Instance.OnDoorOpened();
       StartCoroutine(PanSystem.Instance.PanToDoor(this.transform, panTime));
    }

    public void StartOpen()
    {
        foreach (var part in doorParts)
        {
            part.DoorOpened();
        }
        _audioSource.PlayOneShot(openSound);
        _collider.isTrigger = true; 
        if (_shadowCaster) _shadowCaster.enabled = false;
        _navMeshObstacle.enabled = false;
        _doorOpen = true;
    }


    
        public void CloseDoor()
    {
        foreach (var part in doorParts)
        {
            part.DoorClosed();
        }
        _audioSource.PlayOneShot(closeSound);
        _collider.isTrigger = false; 
        if (_shadowCaster) _shadowCaster.enabled = true;
        _navMeshObstacle.enabled = true;
        MessageManager.Instance.ShowMessage("Door Closed", MessageType.Error);
        gameObject.layer = LayerMask.NameToLayer("Wall");
        _doorOpen = false;

    }

    public bool IsDoorOpened() => _doorOpen;
}

