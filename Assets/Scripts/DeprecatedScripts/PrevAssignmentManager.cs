using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class PrevAssignmentManager : MonoBehaviour
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
        [SerializeField] private PlayerObjectHaver playerObjectHaver;
        
        [Header("Player Children")]
        [SerializeField] private LightSource lightSource;

        public static PrevAssignmentManager Instance { get; private set; }

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
    
    
        public LightSource GetLightSource() => lightSource;
    
        public PlayerLightSourceController GetPlayerLightSourceController() => playerLightSource ;
    
        public PlayerController GetPlayerController() => playerController;
    
        public PlayerHide GetPlayerHide() => playerHide;
    
        public PlayerPickup GetPlayerPickup() => playerPickup;
    
        public PlayerDrag GetPlayerDrag() => playerDrag;
    
        public PlayerLayer GetPlayerLayer() => playerLayer;
    
        public PlayerObjectHaver GetPlayerObjectHaver() => playerObjectHaver;
        public Transform[] GetAllMovementPoints() => allMovementPoints;
        
        public Light2D GetGlobalLight() => globalLight;
        
        public Light2D GetLayer1Light() => layer1Light;
        
        public Light2D GetLayer2Light() => layer2Light;
        
        public Transform GetSpawnPoint() => spawnPoint;
        
        
    
    }
}
