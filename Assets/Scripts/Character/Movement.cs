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

        // Dash system
        private bool _isDashing;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private Vector3 _dashDirection;

        // Recoil system
        private bool _isRecoiling;
        private float _recoilTimer;
        private Vector3 _recoilDirection;

        [NonSerialized] public Vector3 OriginalForwardVector;
        [NonSerialized] public bool IsMoving;
        [NonSerialized] public float DashDistance;
        [NonSerialized] public float DashDuration;
        [NonSerialized] public float DashCooldown;
        [NonSerialized] public float RecoilDistance;
        [NonSerialized] public float RecoilDuration;

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
            if (_isRecoiling)
            {
                PerformRecoil();
            }
            else if (_isDashing)
            {
                PerformDash();
            }
            else
            {
                MovePlayer();
            }

            MovementAnimator();

            if (CompareTag(Constants.PlayerTag)) Rotate(_movementVector);

            // Update cooldown timer
            if (_dashCooldownTimer > 0)
            {
                _dashCooldownTimer -= Time.deltaTime;
            }
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
            IsMoving = false;
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

        public void HandleDash(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_isDashing) return; // Already dashing
            if (_isRecoiling) return; // Can't dash while recoiling
            if (_dashCooldownTimer > 0) return; // On cooldown

            // Get dash direction from current movement or forward
            var dashDir = _movementVector != Vector3.zero ? _movementVector.normalized : transform.forward;

            StartDash(dashDir);
        }

        public void StartDash(Vector3 direction)
        {
            _isDashing = true;
            _dashTimer = DashDuration;
            _dashDirection = direction.normalized;
            _dashCooldownTimer = DashCooldown;

            _animatorCmp.SetBool(Constants.IsDashingAnimatorParam, true);

            var combatCmp = GetComponent<Combat>();

            if (!combatCmp) return;

            combatCmp.CancelAttack();
            combatCmp.StopDefense();
        }

        private void PerformDash()
        {
            _dashTimer -= Time.deltaTime;

            if (_dashTimer <= 0)
            {
                EndDash();
                return;
            }

            // Calculate dash speed
            var dashSpeed = DashDistance / DashDuration;
            var dashOffset = _dashDirection * (dashSpeed * Time.deltaTime);

            if (_agentCmp.isOnNavMesh)
            {
                _agentCmp.Move(dashOffset);
            }
        }

        private void EndDash()
        {
            _isDashing = false;
            _animatorCmp.SetBool(Constants.IsDashingAnimatorParam, false);
        }

        public bool IsDashing()
        {
            return _isDashing;
        }

        /// <summary>
        /// Apply knockback to the character away from a damage source
        /// </summary>
        /// <param name="sourcePosition">The position to recoil away from</param>
        public void ApplyRecoil(Vector3 sourcePosition)
        {
            if (_isDashing) return; // Can't recoil during dash

            // Calculate recoil direction away from source
            Vector3 direction = (transform.position - sourcePosition).normalized;
            direction.y = 0; // Keep on horizontal plane

            if (direction == Vector3.zero)
            {
                // If positions are the same, recoil backwards
                direction = -transform.forward;
            }

            StartRecoil(direction);
        }

        private void StartRecoil(Vector3 direction)
        {
            _isRecoiling = true;
            _recoilDirection = direction;
            _recoilTimer = RecoilDuration;

            // Cancel any current dash
            if (_isDashing)
            {
                EndDash();
            }

            // Stop agent movement
            if (_agentCmp.isOnNavMesh)
            {
                _agentCmp.ResetPath();
            }
        }

        private void PerformRecoil()
        {
            _recoilTimer -= Time.deltaTime;

            if (_recoilTimer <= 0)
            {
                EndRecoil();
                return;
            }

            // Calculate recoil speed with ease-out for smoother stop
            var normalizedTime = 1f - (_recoilTimer / RecoilDuration);
            var easeOut = 1f - (normalizedTime * normalizedTime); // Quadratic ease-out
            var recoilSpeed = (RecoilDistance / RecoilDuration) * easeOut;

            var recoilOffset = _recoilDirection * (recoilSpeed * Time.deltaTime);

            if (_agentCmp.isOnNavMesh)
            {
                _agentCmp.Move(recoilOffset);
            }
        }

        private void EndRecoil()
        {
            _isRecoiling = false;
            _recoilTimer = 0;
        }
    }
}