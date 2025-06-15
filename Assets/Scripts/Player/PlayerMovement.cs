using System;
using DG.Tweening;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private Rigidbody selfRb;
        
        [Header("Movement Settings")]
        [SerializeField] private float slidingSpeed;
        [SerializeField] private Vector3 targetPos;

        private void Awake()
        {
            selfTransform = transform;
            selfRb = GetComponent<Rigidbody>();
        }

        public void SlideHorizontal(float horizontalOffset)
        {
            targetPos = selfTransform.position + new Vector3(horizontalOffset * slidingSpeed, 0, 0);
            targetPos = Vector3.Lerp(selfTransform.position, targetPos, Time.deltaTime);
            selfRb.MovePosition(targetPos);
        }
    }
}