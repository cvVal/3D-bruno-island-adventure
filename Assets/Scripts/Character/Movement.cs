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
        private Animator _animatorCmp;
        
        private Vector3 _movementVector;
        private bool _clampAnimatorSpeedAgain = true;

        [NonSerialized] public Vector3 OriginalForwardVector;
        [NonSerialized] public bool IsMoving;

        private void Awake()
        {
            _agentCmp = GetComponent<NavMeshAgent>();
            _animatorCmp = GetComponentInChildren<Animator>();

            OriginalForwardVector = transform.forward;
        }

        private void Start()
        {
            _agentCmp.updateRotation = false;
        }

        private void Update()
        {
            MovePlayer();
            MovementAnimator();

            if (CompareTag(Constants.PlayerTag)) Rotate(_movementVector);
        }

        private void MovePlayer()
        {
            if (!_agentCmp.isOnNavMesh) return;

            var offset = _movementVector * (Time.deltaTime * _agentCmp.speed);
            _agentCmp.Move(offset);
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
            if (context.performed) IsMoving = true;
            if (context.canceled) IsMoving = false;
            
            var input = context.ReadValue<Vector2>();
            _movementVector = new Vector3(input.x, 0, input.y);
        }

        public void MoveAgentByDestination(Vector3 destination)
        {
            _agentCmp.SetDestination(destination);
            IsMoving = true;
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
            IsMoving = true;
        }

        public void UpdateAgentSpeed(float newSpeed, bool shouldClampSpeed)
        {
            _agentCmp.speed = newSpeed;
            _clampAnimatorSpeedAgain = shouldClampSpeed;
        }

        private void MovementAnimator()
        {
            var speed = _animatorCmp.GetFloat(Constants.SpeedAnimatorParam);
            var smoothening = Time.deltaTime * _agentCmp.acceleration;

            if (IsMoving)
            {
                speed += smoothening;
            }
            else
            {
                speed -= smoothening;
            }

            speed = Mathf.Clamp01(speed);

            if (CompareTag(Constants.EnemyTag) && _clampAnimatorSpeedAgain)
            {
                speed = Mathf.Clamp(speed, 0f, 0.5f);
            }

            _animatorCmp.SetFloat(Constants.SpeedAnimatorParam, speed);
        }
    }
}