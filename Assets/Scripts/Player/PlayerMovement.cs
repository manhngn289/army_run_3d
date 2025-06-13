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
        [SerializeField] private float horizontalSpeed = 10f;
        [SerializeField] private Vector3 targetPos;

        private void Awake()
        {
            selfTransform = transform;
            selfRb = GetComponent<Rigidbody>();
        }

        public void SlideHorizontal(float horizontalOffset)
        {
            print(horizontalOffset);
            Vector3 movementVector = new Vector3(horizontalOffset * Time.deltaTime * horizontalSpeed, 0, 0);
            selfRb.MovePosition(selfTransform.position + movementVector);
        }
    }
}