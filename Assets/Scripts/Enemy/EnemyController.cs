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
        
        // private void Start()
        // {
        //     for(int i = 0; i < amount; i++)
        //     {
        //         UnitEnemyController enemy = ObjectPooler.GetFromPool<UnitEnemyController>(PoolingType.EnemyUnit, selfTransform);
        //         enemy.id = i;
        //         enemy.selfTransform.position = new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0, UnityEngine.Random.Range(10f, 50f));
        //     }
        // }
    }
}