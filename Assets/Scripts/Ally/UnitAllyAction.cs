using System.Collections;
using Framework;
using Framework.ObjectPooling;
using UnityEngine;
using Weapon;

namespace Ally
{
    public class UnitAllyAction : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [SerializeField] private float shootInterval = 2f;
        
        public IEnumerator ShootForwardCoroutine()
        {
            while (true)
            {
                BulletController bullet = ObjectPooler.GetFromPool<BulletController>(PoolingType.NormalBullet);
                bullet.transform.position = transform.position + new Vector3(0, 0.5f, 0.5f);
                bullet.Shoot(transform.forward);
                yield return WaitHelper.GetWait(shootInterval);
            }
        }
    }
}