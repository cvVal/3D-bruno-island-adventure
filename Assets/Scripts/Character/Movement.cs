using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RPG.Character
{
    [RequireComponent(typeof(NavMeshAgent))]
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
            Rotate();
        }

        private void MovePlayer()
        {
            if (!_agent.isOnNavMesh) return;

            var offset = _movementVector * (Time.deltaTime * _agent.speed);
            _agent.Move(offset);
        }

        private void Rotate()
        {
            if (_movementVector == Vector3.zero) return;

            var startRotation = transform.rotation;
            var endRotation = Quaternion.LookRotation(_movementVector);

            // Lerp = Linear interpolation
            transform.rotation = Quaternion.Lerp(
                startRotation,
                endRotation,
                Time.deltaTime * _agent.angularSpeed
            );
        }

        public void HandleMove(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            _movementVector = new Vector3(input.x, 0, input.y);
        }

        public void MoveAgentByDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }
        
        public void StopMovingAgent()
        {
            _agent.ResetPath();
        }
    }
}
