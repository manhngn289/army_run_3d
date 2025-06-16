using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Rigidbody selfRb;

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            selfRb.AddForce(Vector3.back * 30, ForceMode.Impulse);
        }
    }
}