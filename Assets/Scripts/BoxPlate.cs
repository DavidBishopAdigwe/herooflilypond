using UnityEngine;
using UnityEngine.Serialization;

public class BoxPlate : MonoBehaviour
{
    [SerializeField] private Door connectedDoor;
    
    private Collider2D _currentBox;
    private bool _occupied;
    private Collider2D _collider;
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("DraggableBox")) return;
        
        _currentBox = other;
        
        var plateBounds = _collider.bounds;
        var boxBounds = other.bounds;

        bool boxOverlap = (plateBounds.min.x >= boxBounds.min.x && plateBounds.max.x <= boxBounds.max.x && 
                           plateBounds.min.y >= boxBounds.min.y && plateBounds.max.y <= boxBounds.max.y);

        if (boxOverlap != _occupied)
        {
            _occupied = boxOverlap;
            connectedDoor.CheckBoxPads();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == _currentBox)
        {
            _occupied = false;
            _currentBox = null;
            connectedDoor.CheckBoxPads();
        }
    }

    public bool IsPlateOccupied() => _occupied;
}