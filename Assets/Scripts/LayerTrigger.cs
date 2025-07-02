using PlayerScripts;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.Serialization;


public class LayerTrigger : MonoBehaviour
        {
            [SerializeField] private int[] movingMasks;
            [SerializeField] private Light2D layer1Light;
            [SerializeField] private Light2D layer2Light; 
            [SerializeField] private int upLayer; 
            [SerializeField] int downLayer;
            private string _sortingLayer;
            private SpriteRenderer[] _srs;
            private const int PlayerLayer = 9;

            private void OnTriggerExit2D(Collider2D other)
            {
                if (other.gameObject.layer != PlayerLayer) return;
                Vector2 exitPos = other.gameObject.transform.position;
                float direction = exitPos.y - transform.position.y;

                switch (direction)
                {
                    case > 0:
                        _sortingLayer = LayerMask.LayerToName(upLayer);
                        Physics2D.IgnoreLayerCollision(other.gameObject.layer, downLayer, true);
                        Physics2D.IgnoreLayerCollision(other.gameObject.layer, upLayer, false);
                        break;
                    case < 0:
                        _sortingLayer = LayerMask.LayerToName(downLayer);
                        Physics2D.IgnoreLayerCollision(other.gameObject.layer, upLayer, true);
                        Physics2D.IgnoreLayerCollision(other.gameObject.layer, downLayer, false);
                        break;
                }

                if (other.gameObject.TryGetComponent(out SpriteRenderer mainSr))
                {
                    if (_sortingLayer != null) mainSr.sortingLayerName = _sortingLayer;
                }

                if (other.gameObject.TryGetComponent(out PlayerLayer playerLayer))
                {
                    if (_sortingLayer != null) playerLayer.CheckPlayerLayer();
                }

                _srs = other.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (SpriteRenderer sr in _srs)
                {
                    if (_sortingLayer != null) sr.sortingLayerName = _sortingLayer;
                }

               
            }
        }
    