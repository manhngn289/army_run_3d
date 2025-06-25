using System;
using Framework.ObjectPooling;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Enemy Settings")]
        [SerializeField] private int amount;
        public int index;
    }
}