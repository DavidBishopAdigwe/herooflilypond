
using Interfaces;
using UnityEngine;

public class ColliderInteractor: Interactor
{
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            ui.ShowInteractUI();
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            ui.HideInteractUI();
        }
    }

    protected override void Interact()
    {
    }
} 

