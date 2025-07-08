using Managers;
using UnityEngine;

namespace PickableItems
{
    public class PickableOil: PickableItem
    {
        [SerializeField] private float oil;
        
        
        public void AddOilToLamp(LightSource lightSource)
        {
            StartCoroutine(lightSource.GraduallyRefillOil(oil));
        }
    
    }
}
