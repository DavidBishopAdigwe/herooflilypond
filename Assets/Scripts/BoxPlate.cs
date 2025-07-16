using UnityEngine;

public class BoxPlate : MonoBehaviour
{
    [SerializeField] private Door connectedObject;
    
    private Collider2D _currentBox;
    public bool _occupied;
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

        bool boxOverlap = 
            (boxBounds.min.x >= plateBounds.min.x && boxBounds.max.x <= plateBounds.max.x &&
             boxBounds.min.y >= plateBounds.min.y && boxBounds.max.y <= plateBounds.max.y) ||
            (plateBounds.min.x >= boxBounds.min.x && plateBounds.max.x <= boxBounds.max.x &&
             plateBounds.min.y >= boxBounds.min.y && plateBounds.max.y <= boxBounds.max.y);

        if (boxOverlap != _occupied)
        {
            _occupied = boxOverlap;
            connectedObject.CheckBoxPads();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == _currentBox)
        {
            _occupied = false;
            _currentBox = null;
            connectedObject.CheckBoxPads();
        }
    }

    public bool IsPlateOccupied() => _occupied;
}