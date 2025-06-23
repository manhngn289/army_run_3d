using System;
using Enemy;
using UnityEngine;

namespace Player
{
    public class EnemyCheckInRange : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private BoxCollider _selfCollider;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent<UnitEnemyController>(out UnitEnemyController enemy))
            {
                print(enemy.id);
            }
        }
    }
}