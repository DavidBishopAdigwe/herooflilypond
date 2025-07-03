using Managers;
using PlayerScripts;
using UnityEngine;

public class Chest : MonoBehaviour
{
   private PlayerItemTracker _playerObject;
   private MultiLightSource _attachedMultiLightSource;
   private Collider2D _collider;

   private void Awake()
   {
      _collider = GetComponent<Collider2D>();
   }
   private void Start()
   {
      _playerObject = AssignmentManager.Instance.GetPlayerObjectHaver();
      _attachedMultiLightSource = AssignmentManager.Instance.GetLightSource();

     // _attachedLightSource.OnLightToggled += OnLightStateChanged;
   }

   private void OnLightStateChanged(bool lightOn)
   {
      _collider.enabled = lightOn;
   }

   private void OnCollisionEnter2D(Collision2D collision)
   {
      MessageManager.Instance.ShowMessage(_playerObject.PlayerHasKey() ? "CHEST OPENED" : "Chest is locked");

      if (_playerObject.PlayerHasKey())
      {
         GameManager.Instance.WinGame();
      }
   }
}
