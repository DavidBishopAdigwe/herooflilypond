using System.Collections;
using Enums;
using PlayerScripts;
using Singletons.BaseManagers;
using UnityEngine;

namespace Singletons
{
    /// <summary>
    /// Controls the sequence of tutorial messages based on player actions.
    /// Attach this to an empty GameObject in the first gameplay scene.
    /// </summary>
    public class TutorialManager : MonoBehaviourSingleton<TutorialManager>
    {

        [SerializeField] private PlayerController player;
    
        [SerializeField] private float messageCooldown = 2f;
        [SerializeField] private float longMessageCooldown = 6f;

        [SerializeField] private Door door1;
        [SerializeField] private Door door2;
        [SerializeField] private Door finalDoor;

        private PlayerItemTracker _playerItemTracker;
        private PlayerPickup _playerPickup;
        private PlayerDrag _playerDrag;
        private Health _playerHealth;
        private LightSource _light;
        private Coroutine _coroutine;

        private bool _playerMoved;
        private bool _pickedHealth;
        private bool _triedMovingBox;
    

        protected override void Awake()
        {
            base.Awake();
        
            _playerItemTracker = player.GetComponent<PlayerItemTracker>();
            _playerDrag        = player.GetComponent<PlayerDrag>();
            _playerPickup      = player.GetComponentInChildren<PlayerPickup>();
            _light             = player.GetComponentInChildren<LightSource>();
            _playerHealth      = player.GetComponent<Health>();
        }

        private void Start()
        {
            if (!GameManager.Instance.PlayerInTutorial())
            {
                door1.StartOpen();
                door2.StartOpen();
                return;
            } 
            _coroutine =  StartCoroutine(RunTutorial());
        }
    
        private IEnumerator RunTutorial()
        { 
            player .PlayerMoved              += PlayerHasMoved;
            _playerPickup.PickedHealthPotion += PickedHealth;
            _playerDrag.TryDragWithoutRope   += TryMovingBox;
            _playerHealth.TakeDamage(1);

            yield return StartCoroutine(WelcomeStep());
            yield return StartCoroutine(MoveStep());
            yield return StartCoroutine(PickupLampStep());
            yield return StartCoroutine(LampInfo());
            yield return StartCoroutine(PickHealthStep());
            yield return StartCoroutine(TryMoveBoxStep());
            yield return StartCoroutine(FindRopeStep());
            yield return StartCoroutine(DragBoxStep());
            yield return StartCoroutine(OpenDoorStep());
            yield return StartCoroutine(EndTutorialStep()); 
        }

        private IEnumerator WelcomeStep()
        {
            MessageManager.Instance.ShowMessage("Welcome Ferg");
            GameManager.Instance.StartTutorial();
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator MoveStep()
        {
            MessageManager.Instance.ShowMessage("Use WASD keys to move", () => _playerMoved);
            yield return new WaitUntil(() => _playerMoved);
            player.PlayerMoved -= PlayerHasMoved;
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator PickupLampStep()
        {
            MessageManager.Instance.ShowMessage("Pick up the lamp", () => _playerItemTracker.PlayerHasLamp());
            yield return new WaitUntil(() => _playerItemTracker.PlayerHasLamp());
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator LampInfo()
        {
            MessageManager.Instance.ShowMessage("Lamps can reveal hidden items.", longMessageCooldown);
            MessageManager.Instance.ShowMessage("Do be careful though, they make you easier to detect.", longMessageCooldown, MessageType.Warning);
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator PickHealthStep()
        {
            MessageManager.Instance.ShowMessage("Click F to toggle your lamp and look for a health potion with it", () => _pickedHealth);
            yield return new WaitUntil(() => _pickedHealth);
            _playerPickup.PickedHealthPotion -= PickedHealth;
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator TryMoveBoxStep()
        {
            MessageManager.Instance.ShowMessage("That door is blocked by a box. Try to move it.", () => _triedMovingBox);
            door1.OpenDoor();
            yield return new WaitUntil(() => _triedMovingBox);
            _playerDrag.TryDragWithoutRope -= TryMovingBox;
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator FindRopeStep()
        {
            MessageManager.Instance.ShowMessage("Look for a rope, tread carefully though, there's snails around",
                () => _playerItemTracker.PlayerHasRope(),
                MessageType.Warning
            );
            MessageManager.Instance.ShowMessage("You could try hiding in those bushes to avoid being spotted.",
                longMessageCooldown
            );
            yield return new WaitForSeconds(messageCooldown);
            door2.OpenDoor();
            yield return new WaitUntil(() => _playerItemTracker.PlayerHasRope());
        }

        private IEnumerator DragBoxStep()
        {
            MessageManager.Instance.ShowMessage("Alright now you can go drag that box", () => _playerDrag.IsPlayerAttached());
            yield return new WaitUntil(() => _playerDrag.IsPlayerAttached());
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator OpenDoorStep()
        {
            MessageManager.Instance.ShowMessage("Push the box inside and then onto the pressure plate", () => finalDoor.IsDoorOpened());
            yield return new WaitUntil(() => finalDoor.IsDoorOpened());
            yield return new WaitForSeconds(messageCooldown);
        }

        private IEnumerator EndTutorialStep()
        {
            MessageManager.Instance.ShowMessage("Now go, locate the princess' cure, the potion of rainbows", 4, MessageType.Success);
            GameManager.Instance.EndTutorial();
            _light.DisableLight();
            yield break;
        }


        private void PlayerHasMoved()
        {
            _playerMoved = true;
        }

        private void PickedHealth()
        {
            _pickedHealth = true; 
        }

        private void TryMovingBox()
        {
            _triedMovingBox = true;
        }
    }
}
