using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BoxPlate : MonoBehaviour
{
    private float _edgeColliders;
    private bool _occupied;
    [SerializeField] private Door connectedObject;

    public void BoxHitEdge()
    {
        Debug.Log("Box hit edge");
        _edgeColliders++;
        CheckOccupationState();
    }

    public void BoxExitEdge()
    {
        Debug.Log("Box exit edge");
        _edgeColliders--;
        CheckOccupationState();
    }
    
    private void CheckOccupationState()
    {
        _occupied = _edgeColliders >= 4;
        connectedObject.PublicChecker();
        
    }

    public bool ReturnOccupationState() => _occupied;

}
