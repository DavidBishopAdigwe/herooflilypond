using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerScripts.PlayerInteractor
{
    /// <summary>
    /// Base class for all player scripts that interact with objects
    /// </summary>
    public abstract class Interactor : MonoBehaviour
    {
        protected virtual void Start()
        {
            InputReader.Instance.InteractPerformed += OnInteractKeyClicked;
        }



        protected abstract void OnInteractKeyClicked(InputAction.CallbackContext obj);
    

        protected virtual void OnDestroy()
        {
            InputReader.Instance.InteractPerformed -= OnInteractKeyClicked;
        }







    }
}


