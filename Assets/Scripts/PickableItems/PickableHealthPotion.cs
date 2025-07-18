using UnityEngine;

namespace PickableItems
     {
         public class PickableHealthPotion: PickableItem
         { 
             [SerializeField] private int hpToAdd;
             

             public void AddHp(Health hp)
             {
                 if (hp.IsMax()) return;
                 hp.AddHealth(hpToAdd);
             }
             
         }
     }
