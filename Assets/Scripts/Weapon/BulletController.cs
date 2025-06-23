using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Framework;
using Framework.ObjectPooling;
using UnityEngine;

namespace Weapon
{
    public class BulletController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Rigidbody selfRb;
        
        [Header("Bullet Settings")]
        [SerializeField] private float shootForce = 20f;
        public int index;

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
        }

        public void Shoot(Vector3 direction)
        {
            selfRb.velocity = Vector3.zero;
            selfRb.AddForce(direction * shootForce, ForceMode.Impulse);
            StartCoroutine(CoutDown());
        }

        private IEnumerator CoutDown()
        {
            yield return WaitHelper.GetWait(3f);
            if(gameObject.activeSelf)
                ObjectPooler.ReturnToPool(PoolingType.NormalBullet, this);
        }
    }
}