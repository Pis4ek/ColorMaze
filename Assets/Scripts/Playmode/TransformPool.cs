using System.Collections.Generic;
using UnityEngine;

namespace Playmode
{
    public class TransformPool
    {
        private Stack<Transform> _pool = new();
        private GameObject _cellPrefab;
        private Transform _fillersContainer;

        public TransformPool(GameObject prefab, Transform fillersContainer, int startPoolCapacity)
        {
            _cellPrefab = prefab;
            _fillersContainer = fillersContainer;
            if (startPoolCapacity < 0) startPoolCapacity = 10;

            for (int i = 0; i < startPoolCapacity; i++)
            {
                Create();
            }
        }

        public Transform Get()
        {
            Transform obj;
            if( _pool.Count == 0 )
            {
                obj = Create();
            }
            else
            {
                obj = _pool.Pop();
            }
            return obj;
        }

        public void Add(Transform obj)
        {
            _pool.Push(obj);
        }

        private Transform Create()
        {
            var obj = Object.Instantiate(_cellPrefab, _fillersContainer);
            obj.SetActive(false);
            return obj.transform;
        }
    }
}