using System;
using Framework.ObjectPooling;
using UnityEngine;
using Weapon;

namespace Manager
{
    public class PoolingManager : MonoBehaviour
    {
        [SerializeField] private BulletController bulletPrefab;
        private void Awake()
        {
            ObjectPooler.SetUpPool(PoolingType.NormalBullet, 50, bulletPrefab);
        }
    }
}