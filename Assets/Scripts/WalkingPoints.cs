using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkingPoints : MonoBehaviour
{

    private enum WalkingPointType
    {
        
    }
    private string _currentLayer;

    private void Start()
    {
        _currentLayer = LayerMask.LayerToName(gameObject.layer);
    }
    
    public string GetCurrentLayer()
    {
        return _currentLayer;
    }
    
    


}
