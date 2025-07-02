 using UnityEngine;

 namespace PlayerScripts
 {
     public class PlayerObjectHaver: MonoBehaviour
     {
         // To control and know what objects the player currently has
        
         private bool _hasKey;
         private bool _hasLamp;
         private bool _hasRope;

         public void PickedLamp() => _hasLamp = true;
         public void PickedKey() => _hasKey = true;
         public void PickedRope() => _hasRope = true;
        
         public bool PlayerHasKey() =>_hasKey;
        
         public bool PlayerHasLamp() =>_hasLamp;
         
         public bool PlayerHasRope() =>_hasRope;
         
        
        
     }
 }
