using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class ObjectPooler
    {
        private static int capacity = Enum.GetValues(typeof(PoolingType)).Length;
        
        private static List<Component>[] _poolList = new List<Component>[capacity];
        
        
    }
}