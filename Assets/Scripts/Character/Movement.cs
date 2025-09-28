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
        private NavMeshAgent _agentCmp;
        private Vector3 _movementVector;

        [NonSerialized] public Vector3 OriginalForwardVector;

        private void Awake()
        {
            _agentCmp = GetComponent<NavMeshAgent>();

            OriginalForwardVector = transform.forward;
        }

        private void Start()
        {
            _agentCmp.updateRotation = false;
        }

        private void Update()
        {
            MovePlayer();

            if (CompareTag(Constants.PlayerTag)) Rotate(_movementVector);
        }

        private void MovePlayer()
        {
            if (!_agentCmp.isOnNavMesh) return;

            var offset = _movementVector * (Time.deltaTime * _agentCmp.speed);
            MoveAgentByOffset(offset);
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
                Time.deltaTime * _agentCmp.angularSpeed
            );
        }

        public void HandleMove(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            _movementVector = new Vector3(input.x, 0, input.y);
        }

        public void MoveAgentByDestination(Vector3 destination)
        {
            _agentCmp.SetDestination(destination);
        }

        public void StopMovingAgent()
        {
            _agentCmp.ResetPath();
        }

        public bool HasReachedDestination()
        {
            if (_agentCmp.pathPending) return false;

            if (_agentCmp.remainingDistance > _agentCmp.stoppingDistance) return false;

            return !_agentCmp.hasPath && _agentCmp.velocity.sqrMagnitude == 0f;
        }

        public void MoveAgentByOffset(Vector3 offset)
        {
            _agentCmp.Move(offset);
        }

        public void UpdateAgentSpeed(float newSpeed)
        {
            _agentCmp.speed = newSpeed;
        }
    }
}