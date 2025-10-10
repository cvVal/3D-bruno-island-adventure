using System;
using System.Collections.Generic;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Character
{
    public class Combat : MonoBehaviour
    {
        // track already damaged targets per attack
        private readonly HashSet<Health> _hitTargets = new();

        private Animator _animatorCmp;
        private BubbleEvent _bubbleEventCmp;
        private AudioSource _audioSourceCmp;
        private bool _isAttacking;
        private bool _isDefending;

        [Header("Audio Settings")] public AudioClip attackClip;

        [Header("Defense Settings")] [Range(0f, 1f)]
        public float defenseReduction = 0.5f; // 50% damage reduction by default

        [NonSerialized] public float Damage;
        [NonSerialized] public float AttackSpeed = 1f;

        private void Awake()
        {
            _animatorCmp = GetComponentInChildren<Animator>();
            _bubbleEventCmp = GetComponentInChildren<BubbleEvent>();
            _audioSourceCmp = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _bubbleEventCmp.OnBubbleStartAttack += HandleBubbleStartAttack;
            _bubbleEventCmp.OnBubbleCompleteAttack += HandleBubbleCompleteAttack;
            _bubbleEventCmp.OnBubbleHit += HandleBubbleHit;
        }

        private void OnDisable()
        {
            _bubbleEventCmp.OnBubbleStartAttack -= HandleBubbleStartAttack;
            _bubbleEventCmp.OnBubbleCompleteAttack -= HandleBubbleCompleteAttack;
            _bubbleEventCmp.OnBubbleHit -= HandleBubbleHit;
        }

        public void HandleAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_isDefending) return; // Can't attack while defending
            
            // Can't attack while dashing
            var movementCmp = GetComponent<Movement>();
            if (movementCmp && movementCmp.IsDashing()) return;

            StartAttack();
        }

        public void HandleDefense(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Can't defend while dashing
                var movementCmp = GetComponent<Movement>();
                if (movementCmp && movementCmp.IsDashing()) return;
                
                StartDefense();
            }
            else if (context.canceled)
            {
                StopDefense();
            }
        }

        public void StartAttack()
        {
            if (_isAttacking) return;

            _animatorCmp.SetFloat(Constants.SpeedAnimatorParam, 0);

            // Set the attack animation speed based on attackSpeed
            _animatorCmp.SetFloat(Constants.AttackSpeedAnimatorParam, AttackSpeed);

            _animatorCmp.SetTrigger(Constants.AttackAnimatorParam);

            if (attackClip && _audioSourceCmp)
                _audioSourceCmp.PlayOneShot(attackClip);
        }

        private void StartDefense()
        {
            if (_isAttacking) return; // Can't defend while attacking

            _isDefending = true;
            _animatorCmp.SetFloat(Constants.SpeedAnimatorParam, 0);
            _animatorCmp.SetBool(Constants.IsDefendingAnimatorParam, true);
        }

        public void StopDefense()
        {
            if (!_isDefending) return;
            
            _isDefending = false;
            _animatorCmp.SetBool(Constants.IsDefendingAnimatorParam, false);
        }

        private void HandleBubbleStartAttack()
        {
            _isAttacking = true;
        }

        private void HandleBubbleCompleteAttack()
        {
            _isAttacking = false;
            _hitTargets.Clear();
        }

        private void HandleBubbleHit()
        {
            RaycastHit[] targets = Physics.BoxCastAll(
                transform.position + transform.forward,
                transform.localScale / 2,
                transform.forward,
                transform.rotation,
                1f
            );

            foreach (RaycastHit target in targets)
            {
                // skip self / same-tag entities
                if (CompareTag(target.transform.tag)) continue;

                var healthCmp = target.transform.gameObject.GetComponent<Health>();
                if (!healthCmp) continue;

                // already damaged this swing
                if (_hitTargets.Contains(healthCmp)) continue;

                // Apply damage
                healthCmp.TakeDamage(Damage);
                _hitTargets.Add(healthCmp);
                
                // Apply recoil to the target
                var targetMovement = target.transform.GetComponent<Movement>();
                if (targetMovement)
                    targetMovement.ApplyRecoil(transform.position);
            }
        }

        public void CancelAttack()
        {
            _animatorCmp.ResetTrigger(Constants.AttackAnimatorParam);
        }

        public float GetDefenseReduction()
        {
            return _isDefending ? defenseReduction : 0f;
        }
    }
}