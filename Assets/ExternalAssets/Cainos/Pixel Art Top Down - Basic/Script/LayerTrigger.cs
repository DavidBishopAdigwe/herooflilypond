using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Cainos.PixelArtTopDown_Basic
{
    //when object exit the trigger, put it to the assigned layer and sorting layers
    //used in the stair objects for player to travel between layers
    public class LayerTrigger : MonoBehaviour
    {
        [SerializeField] private int[] movingMasks; 
        [SerializeField] private Light2D layer1Light;
        [SerializeField] private Light2D layer2Light;
        public int layerToIgnore;
        public int layerToCollide;
        public string layer;
        public string sortingLayer;
        private SpriteRenderer[] _srs;

        private void OnTriggerExit2D(Collider2D other)
        {


            if (other.gameObject.TryGetComponent(out SpriteRenderer mainSr))
            {
                mainSr.sortingLayerName = sortingLayer;
               /* switch (sortingLayer)
                {
                    case "Layer 1":
                        layer1Light.intensity = 0.06f;
                        layer2Light.intensity = 0.03f;
                        break;
                    case "Layer 2":
                        layer1Light.intensity = 0.03f;
                        layer2Light.intensity = 0.06f;
                        break;
                } */
            }

            if (other.gameObject.TryGetComponent(out PlayerLayer playerLayer))
            {
                playerLayer.CheckPlayerLayer();
            }
            _srs = other.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in _srs)
            {
                sr.sortingLayerName = sortingLayer;
            }

            foreach (int mask in movingMasks)
            {
                Physics2D.IgnoreLayerCollision(mask, layerToIgnore, true);
                Physics2D.IgnoreLayerCollision(mask, layerToCollide, false);
            }
        }



    }
}
