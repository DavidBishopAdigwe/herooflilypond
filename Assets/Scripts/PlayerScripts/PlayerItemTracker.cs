 using DataPersistence;
 using DataPersistence.Data;
 using Interfaces;
 using UnityEngine;

 namespace PlayerScripts
 {
     public class PlayerItemTracker: MonoBehaviour
     {
        
         private bool _hasLamp;
         private bool _hasRope;

         public void PickedLamp() => _hasLamp = true;
         public void PickedRope() => _hasRope = true;
         
         public bool PlayerHasLamp() =>_hasLamp;
         
         public bool PlayerHasRope() =>_hasRope;


    
     }
 }
