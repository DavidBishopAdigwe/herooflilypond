using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CollisionInteractor : Interactor
{
    
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out UIDisplayableObject ui))
        {
            ui.ShowInteractUI();
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out UIDisplayableObject ui))
        {
            ui.HideInteractUI();
        }
    }
}