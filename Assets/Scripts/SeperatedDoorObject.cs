using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


public class SeperatedDoorObject: MonoBehaviour
{
    
    private Collider2D _collider;
    private Animator _animator;
    
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    public void DoorOpened()
    {
        _animator.SetTrigger("Opened");
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void DoorClosed()
    {
        gameObject.layer = LayerMask.NameToLayer("Wall");
        _animator.SetTrigger("Closed");
    }
    

    public void SetToOpen()
    {
        _collider.enabled = false;
    }

    public void SetToClosed()
    {
       _collider.enabled = true;
    }
} 

