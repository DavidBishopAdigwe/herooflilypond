using Managers;
using UnityEngine;

namespace PickableItems
{
    public class PickableLampOil: PickableItem
    {
        [SerializeField] private float oil;
        
        
        public void AddOilToLamp(LightSource lightSource)
        {
            lightSource.OilRefill(oil);
       
        }
    
    }
}
