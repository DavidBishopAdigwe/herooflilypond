using UnityEngine;

namespace PickableItems
     {
         public class PickableHealthPotion: PickableObject
         { 
             [SerializeField] private int hpToAdd;

             public void AddHp(ref Health hp)
             {
                 hp.AddHealth(hpToAdd);
             }
         }
     }
