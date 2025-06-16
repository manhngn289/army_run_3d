using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private float shootForce = 10f;

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(KeySave.EnemyTag))
            {
                Destroy(other.gameObject);
                ObjectPooler.ReturnToPool(PoolingType.NormalBullet, this);
            }
        }

        public void Shoot(Vector3 direction)
        {
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