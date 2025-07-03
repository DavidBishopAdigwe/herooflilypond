using UnityEngine;

namespace PickableItems
     {
         public class PickableHealthPotion: PickableItem
         { 
             [SerializeField] private int hpToAdd;

             public void AddHp(ref Health hp)
             {
                 hp.AddHealth(hpToAdd);
             }
         }
     }
