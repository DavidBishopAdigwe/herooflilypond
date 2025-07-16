using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DoorSystems : GameObjectSingleton<DoorSystems>
{

    [SerializeField] private GameObject player;
    private CinemachineCamera _playerCamera;
    private PlayerDrag _playerDrag;
    private PlayerController _playerController;

    private void Start()
    {
        _playerCamera = player.GetComponentInChildren<CinemachineCamera>();
        _playerController = player.GetComponent<PlayerController>();
        _playerDrag = player.GetComponent<PlayerDrag>();
    }

    public IEnumerator PanToDoor(Transform doorTransform, float panTime = 1f)
    {
        var cameraTarget = new CameraTarget();
        OnDoorOpened();
        _playerController.UnsubscribeInputs();
        cameraTarget.TrackingTarget = doorTransform;
        _playerCamera.Target = cameraTarget;
        yield return new WaitForSeconds(panTime);
        cameraTarget.TrackingTarget = player.transform;
        _playerCamera.Target = cameraTarget;
        _playerController.SubscribeInputs();
    }

    public void OnDoorOpened()
    {
        _playerDrag.DetachFromObject();
    }

}