using System.Collections;
using Singletons.BaseManagers;
using Unity.Cinemachine;
using UnityEngine;

namespace Singletons
{
    public class PanSystem : MonoBehaviourSingleton<PanSystem>
    {

        [SerializeField] private GameObject player;
        private CinemachineCamera _playerCamera;
        private PlayerDrag _playerDrag;

        private void Start()
        {
            _playerCamera = player.GetComponentInChildren<CinemachineCamera>(); 
            _playerDrag = player.GetComponent<PlayerDrag>();
        }

        public IEnumerator PanToDoor(Transform doorTransform, float panTime = 1f)
        {
            var cameraTarget = new CameraTarget();
            OnDoorOpened();
            InputReader.Instance.UnsubscribeMoveAction();
        
            cameraTarget.TrackingTarget = doorTransform;
            _playerCamera.Target = cameraTarget;
        
            yield return new WaitForSeconds(panTime);
        
            cameraTarget.TrackingTarget = player.transform;
            _playerCamera.Target = cameraTarget;
            InputReader.Instance.SubscribeMoveAction();
        }

        public void OnDoorOpened()
        {
            _playerDrag.DetachFromObject();
        }

    }
}