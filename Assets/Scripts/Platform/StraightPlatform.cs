using System;
using UnityEngine;

namespace Platform
{
    public class StraightPlatform : MonoBehaviour
    {
        [Header("Platform Settings")]
        [SerializeField] private PlatformLevelType platformLevelType;
        [SerializeField] private float speed = 5f;

        private void Update()
        {
            transform.position += Vector3.back * (speed * Time.deltaTime);
        }
    }
}