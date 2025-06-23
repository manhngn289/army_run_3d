using System;
using System.Collections;
using Framework;
using UnityEngine;

namespace Ally
{
    public class UnitAllyController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Rigidbody selfRb;
        
        [Header("Unit Ally Components")]
        [SerializeField] private UnitAllyAction action;

        private void Awake()
        {
            selfRb = GetComponent<Rigidbody>();
            
            action = GetComponent<UnitAllyAction>();
        }

        private void Start()
        {
            // StartCoroutine(action.ShootForwardCoroutine());
        }
    }
}