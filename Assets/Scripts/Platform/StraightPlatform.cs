using System;
using UnityEngine;

namespace Platform
{
    public class StraightPlatform : MonoBehaviour
    {
        [Header("Platform Settings")]
        [SerializeField] private PlatformLevelType platformLevel;
        [SerializeField] private Rigidbody selfRb;
        [SerializeField] private float speed = 5f;

        public Vector3 Velocity
        {
            set
            {
                if (selfRb != null)
                {
                    selfRb.velocity = value;
                }
            }
        }

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Velocity = Vector3.back * speed;
        }
    }
}