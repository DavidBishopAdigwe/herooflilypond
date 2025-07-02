using UnityEngine;
using UnityEngine.Serialization;

namespace DeprecatedScripts
{
    public class DeprecatedLightActivatable : MonoBehaviour, IActivatable
    {
        [SerializeField] private LayerMask playerLight;
        private SpriteRenderer _parentSpriteRenderer;
        [FormerlySerializedAs("_parentCollider")] [SerializeField]  private Collider2D parentCollider;
        private Collider2D _collider;
    
        private void Start()
        {
            _parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();
            Deactivate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PlayerLight"))
            {
                Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("PlayerLight") )
            {
                Deactivate();
            }
        }
    

        public void Activate()
        {
            _parentSpriteRenderer.enabled = true;
            parentCollider.enabled = true;
        }

        public void Deactivate()
        {
            _parentSpriteRenderer.enabled = false;  
            parentCollider.enabled = false;
        }
    
    }
}
