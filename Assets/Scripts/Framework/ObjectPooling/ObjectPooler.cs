using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.ObjectPooling
{
    public static class ObjectPooler
    {
        private static int capacity = Enum.GetValues(typeof(PoolingType)).Length;
        
        private static List<Component>[] _poolList = new List<Component>[capacity];

        private static readonly Transform poolParent = GameObject.FindGameObjectWithTag(KeySave.PoolParent).transform;

        public static void SetUpPool<T>(PoolingType type, int size, T prefab) where T : Component
        {
            if (size <= 1)
                throw new Exception("Pool size must be greater than 1");
            
            _poolList[(byte)type] = new List<Component>(size);

            for (int i = 0; i < size; i++)
            {
                T instance = GameObject.Instantiate(prefab);
                ReturnToPool(type, instance);
            }
        }

        public static T GetFromPool<T>(PoolingType type, Transform parent = null) where T : Component
        {
            if (_poolList[(byte)type].Count > 1)
            {
                Component instance = _poolList[(byte)type][_poolList[(byte)type].Count - 1];
                _poolList[(byte)type].RemoveAt(_poolList[(byte)type].Count - 1);

                instance.gameObject.SetActive(true);
                instance.transform.SetParent(parent);

                return (T)instance;
                
            }

            T newInstance = GameObject.Instantiate((T)_poolList[(byte)type][0], parent);
            newInstance.gameObject.SetActive(true);
            
            return newInstance;
        }

        public static void ReturnToPool<T>(PoolingType type, T instance) where T : Component
        {
            PreprocessData(instance);
            _poolList[(byte)type].Add(instance);
        }

        private static void PreprocessData<T>(T instance) where T : Component
        {
            instance.transform.SetParent(poolParent);
            instance.gameObject.SetActive(false);

            instance.transform.position = Vector3.zero;
            instance.transform.eulerAngles = Vector3.zero;
            instance.transform.localScale = Vector3.one;
        }
    }
}