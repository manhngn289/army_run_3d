using System;
using Framework;
using UnityEngine;

namespace Ally
{
    public class UnitAllyController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Rigidbody selfRb;

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(KeySave.EnemyTag))
            {
                Destroy(other.gameObject);
            }
        }
    }
}