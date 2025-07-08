using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BoxPlate : MonoBehaviour
{
    private float _edgeColliders;
    private bool _occupied;
    [SerializeField] private Door connectedObject;

    public void BoxEnterEdge()
    {
        _edgeColliders++;
        CheckOccupationState();
    }

    public void BoxExitEdge()
    {
        _edgeColliders--;
        CheckOccupationState();
    }
    
    private void CheckOccupationState()
    {
        _occupied = _edgeColliders >= 4;
        connectedObject.CheckBoxPads();
        
    }

    public bool IsPlateOccupied() => _occupied;

}
