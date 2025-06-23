using System;
using Framework.ObjectPooling;
using UnityEngine;
using Weapon;

namespace Enemy
{
    public class UnitEnemyController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Rigidbody selfRb;
        public Transform selfTransform;

        [Header("Enemy Data")]
        public int id;

        [Header("Enemy Collision")] 
        public bool isHit;

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
            selfTransform = transform;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out BulletController bullet) && isHit)
            {
                print($"Enemy {id} hit by bullet");
                ObjectPooler.ReturnToPool(PoolingType.EnemyUnit, this);
                ObjectPooler.ReturnToPool(PoolingType.NormalBullet, bullet);
            }
        }

        private void Start()
        {
            // selfRb.AddForce(Vector3.back * 30, ForceMode.Impulse);
        }
    }
}