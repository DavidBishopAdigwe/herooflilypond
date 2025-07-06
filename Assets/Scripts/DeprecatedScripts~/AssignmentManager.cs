using DeprecatedScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Managers
{
    public class AssignmentManager : MonoBehaviour
    {
    
        // Only for assignment to global objects, player scripts and player children.
    
        [Header("Global Objects")]
        [SerializeField] private Transform[] allMovementPoints; 
        [SerializeField] private Light2D globalLight;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject messageParent;
        
        // Temp map specific layer lights, mainly to darken the layer player is not on.
        [SerializeField] private Light2D layer1Light;
        [SerializeField] private Light2D layer2Light;
        
        [Header("Player Scripts")]
        [SerializeField] private PlayerLightSourceController playerLightSource;
        [SerializeField] private PlayerController playerController; 
        [SerializeField] private PlayerHide playerHide;
        [SerializeField] private PlayerPickup playerPickup; 
        [SerializeField] private PlayerDrag playerDrag;
        [SerializeField] private PlayerLayer playerLayer;
        [FormerlySerializedAs("playerObjectHaver")] [SerializeField] private PlayerItemTracker playerItemTracker;
        
        [FormerlySerializedAs("lightSource")]
        [Header("Player Children")]
        [SerializeField] private MultiLightSource multiLightSource;

        public static AssignmentManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
    
        public MultiLightSource GetLightSource() => multiLightSource;
        
        public PlayerItemTracker GetPlayerObjectHaver() => playerItemTracker;
        
        public Light2D GetGlobalLight() => globalLight;
        
        public Light2D GetLayer1Light() => layer1Light;
        
        public Light2D GetLayer2Light() => layer2Light;
        
    
    }
}
