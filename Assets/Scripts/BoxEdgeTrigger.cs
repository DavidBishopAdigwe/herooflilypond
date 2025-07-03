using UnityEngine;
    public class BoxEdgeTrigger:MonoBehaviour
    {
        private BoxPlate _plate;

        private void Start()
        {
            _plate = GetComponentInParent<BoxPlate>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("DraggableBox"))
            {
                _plate.BoxEnterEdge();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("DraggableBox"))
            {
                _plate.BoxExitEdge();
            }
        }
        
        
    }
