using System;
using RPG.Utility;
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

        [NonSerialized] public Vector3 OriginalForwardVector;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            OriginalForwardVector = transform.forward;
        }

        private void Start()
        {
            _agent.updateRotation = false;
        }

        private void Update()
        {
            MovePlayer();

            if (CompareTag(Constants.PlayerTag)) Rotate(_movementVector);
        }

        private void MovePlayer()
        {
            if (!_agent.isOnNavMesh) return;

            var offset = _movementVector * (Time.deltaTime * _agent.speed);
            _agent.Move(offset);
        }

        public void Rotate(Vector3 newForwardVector)
        {
            if (newForwardVector == Vector3.zero) return;

            var startRotation = transform.rotation;
            var endRotation = Quaternion.LookRotation(newForwardVector);

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

        public bool HasReachedDestination()
        {
            if (_agent.pathPending) return false;

            if (_agent.remainingDistance > _agent.stoppingDistance) return false;

            return !_agent.hasPath && _agent.velocity.sqrMagnitude == 0f;
        }

        public void MoveAgentByOffset(Vector3 offset)
        {
            _agent.Move(offset);
        }

        public void UpdateAgentSpeed(float newSpeed)
        {
            _agent.speed = newSpeed;
        }
    }
}