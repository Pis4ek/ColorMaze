using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        public TKey Key; 
        public TValue Value;
    }
}