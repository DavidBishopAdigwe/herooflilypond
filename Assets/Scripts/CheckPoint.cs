using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject blackBox;
    
    private string _currentLayer;
    
    private void Start()
    {
        GetCurrentLayer();
    }

    public string SwapPlayerLayer()
    {
        return _currentLayer; 
    }

    private void GetCurrentLayer()
    {
        _currentLayer = LayerMask.LayerToName(gameObject.layer);
    }
}
