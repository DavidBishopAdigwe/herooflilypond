
using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthObject: MonoBehaviour
{
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private Sprite emptySprite;
    private Animator _animator;
    private Image _image;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
    }

    public void DamagedAnimation()
    {
        _animator.SetTrigger("Damaged");
    }

    public void SetHeartToEmpty()
    {
        _image.sprite = emptySprite;
    }
} 

