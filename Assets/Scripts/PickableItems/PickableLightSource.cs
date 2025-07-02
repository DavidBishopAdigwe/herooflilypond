using System;
using Enums;
using Managers;
using UnityEngine;

namespace PickableItems
{
    public class PickableLightSource : PickableObject
    {
        public LightSourceType lightSourceType = LightSourceType.None;
        
        private LightSource _attachedLightSource;

        protected override void Start()
        {
            base.Start();
        }
        
    }
}



    

