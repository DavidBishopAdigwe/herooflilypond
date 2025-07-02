using Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PlayerScripts
{
    public class PlayerLayer:MonoBehaviour
    {

        [SerializeField] private float activeLightIntensity;
        [SerializeField] private float inactiveLightIntensity;
        [SerializeField] private float globalLightIntensity;
        
        private Light2D _layer1Light; 
        private Light2D _layer2Light; 
        private Light2D _globalLight; 
        
        private const int PlayersLayer = 9;
        private const int Layer1 = 20;
        private const int Layer2 = 21;
        
        private SpriteRenderer _playerSprite;
        

        private void Awake()
        {
            _playerSprite = GetComponent<SpriteRenderer>();
            
        }
        private void Start()
        {
          _layer1Light = AssignmentManager.Instance.GetLayer1Light();
          _layer2Light = AssignmentManager.Instance.GetLayer2Light(); 
          _globalLight = AssignmentManager.Instance.GetGlobalLight();
            CheckPlayerLayer();
            _globalLight.intensity = globalLightIntensity;
        }
        

        public void CheckPlayerLayer()
        {
            if (_playerSprite == null) return;
            int sortingLayerId = _playerSprite.sortingLayerID;

            if (_playerSprite.sortingLayerName == LayerMask.LayerToName(Layer1))
            {
                Physics2D.IgnoreLayerCollision(PlayersLayer, Layer1, false);
                Physics2D.IgnoreLayerCollision(PlayersLayer, Layer2, true);
                _layer1Light.intensity = activeLightIntensity;
                _layer2Light.intensity = inactiveLightIntensity;

            }
            else if (_playerSprite.sortingLayerName == LayerMask.LayerToName(Layer2))
            {
                Physics2D.IgnoreLayerCollision(PlayersLayer, Layer2, false);
                Physics2D.IgnoreLayerCollision(PlayersLayer, Layer1, true);
                _layer2Light.intensity = activeLightIntensity;
                _layer1Light.intensity = inactiveLightIntensity; ;
            } 
            // _attachedLight.m_ApplyToSortingLayers = new int[] {SortingLayer.NameToID("Default"), sortingLayerId };
           
            // m_ApplytoSortinglayers reverts back to private when exported to other computers, looking for new solution

        }
        

    }
    
    
    
}