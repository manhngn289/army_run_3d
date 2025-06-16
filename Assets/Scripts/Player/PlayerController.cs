using System;
using Ally;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Components")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInput playerInput;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            playerInput.HandleInput();
        }

        private void FixedUpdate()
        {
            if (playerInput.CanSlideHorizontal)
            {
                playerMovement.SlideHorizontal(playerInput.SlidingDirection);
            }
        }
    }
}