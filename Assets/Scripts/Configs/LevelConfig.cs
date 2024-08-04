using System;
using System.Collections;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class LevelConfig
    {
        public Vector2Int StartPoint;
        public int CoinsCount;
        public int[,] Map;
    }
}