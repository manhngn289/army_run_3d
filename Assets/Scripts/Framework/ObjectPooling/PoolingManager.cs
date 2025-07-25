﻿using System;
using Enemy;
using Framework.ObjectPooling;
using UnityEngine;
using Weapon;

namespace Manager
{
    public class PoolingManager : MonoBehaviour
    {
        [Header("Pooling Settings")]
        public static int MaxBullets = 100;
        public static int MaxEnemies = 100;
        
        [Header("Prefabs")]
        [SerializeField] private BulletController bulletPrefab;
        [SerializeField] private UnitEnemyController unitEnemyPrefab;
        
        private void Awake()
        {
            ObjectPooler.SetUpPool(PoolingType.NormalBullet, MaxBullets, bulletPrefab);
            ObjectPooler.SetUpPool(PoolingType.EnemyUnit, MaxEnemies, unitEnemyPrefab);
        }
    }
}