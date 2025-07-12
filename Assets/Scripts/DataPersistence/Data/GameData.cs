using System;
using System.Collections.Generic;

namespace DataPersistence.Data
{
    [Serializable]
    public class GameData
    {
        public float[] playerPosition;
        public float lightIntensity;
        public float lightRadius;
        public float lightInnerRadius;
        public float lightTimeRemaining;
        public int[] res;
        public bool playerHasLamp;

        public GameData()
        {
            playerPosition = new float[3];
            res = new int[2];
            playerPosition[0] = 0;
            playerPosition[1] = 0;
            playerPosition[2] = 0;
            res[0] = 1920;
            res[1] = 1080;
            lightIntensity = 0;
            lightTimeRemaining = 0;
            lightRadius = 0;
            lightInnerRadius = 0;
            playerHasLamp = false;
        }
        
        
        
    }
}