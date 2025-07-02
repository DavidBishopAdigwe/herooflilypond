using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.AI;

public class Door: MonoBehaviour
    {
        [SerializeField] private BoxPlate[] padsToOpen;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D collider2;
        [SerializeField] private NavMeshObstacle navMeshObstacle;
        [SerializeField] [Multiline] private string messageOnLocked;
        [SerializeField] [Multiline] private string messageOnUnlocked;

        private bool _open;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider2 = GetComponent<Collider2D>();
            navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && !_open )
            {
                MessageManager.Instance.ShowMessage(messageOnLocked);
            }
        }

        private void CheckBoxPads()
        {
            bool allPlatesOccupied = padsToOpen.All(plate => plate.ReturnOccupationState());

            if (allPlatesOccupied)
            {
                
                spriteRenderer.enabled = false;
                collider2.enabled = false;
                navMeshObstacle.enabled = false;
                _open = true;
                MessageManager.Instance.ShowMessage(messageOnUnlocked);
            }
            else
            {
                spriteRenderer.enabled = true;
                collider2.enabled = true;
                navMeshObstacle.enabled = true;
                _open = false;
            }
            
        }

        public void PublicChecker()
        {
            CheckBoxPads();
        }
    }
