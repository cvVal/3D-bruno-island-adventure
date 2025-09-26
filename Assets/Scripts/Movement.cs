using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RPG.Character
{
    public class Movement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Vector3 _movementVector;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            if (_agent.isOnNavMesh)
            {
                _agent.Move(_movementVector);
            }
        }

        public void HandleMove(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            _movementVector = new Vector3(input.x, 0, input.y);
            
            print(_movementVector);
        }
    }
}
