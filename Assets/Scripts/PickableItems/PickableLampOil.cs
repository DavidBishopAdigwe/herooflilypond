using Managers;
using UnityEngine;

namespace PickableItems
{
    public class PickableLampOil: PickableObject
    {
        [SerializeField] private float oil;
        
        
        public void AddOilToLamp(Lamp lamp)
        {
            lamp.OilRefill(oil);
       
        }
    
    }
}
