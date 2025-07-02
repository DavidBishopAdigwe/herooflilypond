using System;
using System.Collections.Generic;

namespace DataPersistence.Data
{
    [Serializable]
    public class GameData
    {
        public float[] playerPosition;

        public GameData()
        {
            playerPosition = new float[3];
            playerPosition[0] = 0;
            playerPosition[1] = 0;
            playerPosition[2] = 0;
        }
    }
}