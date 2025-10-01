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

        [NonSerialized] public float Damage;
        [NonSerialized] public bool IsAttacking;

        private void Awake()
        {
            _animatorCmp = GetComponentInChildren<Animator>();
            _bubbleEventCmp = GetComponentInChildren<BubbleEvent>();
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

            StartAttack();
        }

        public void StartAttack()
        {
            if (IsAttacking) return;

            _animatorCmp.SetFloat(Constants.SpeedAnimatorParam, 0);
            _animatorCmp.SetTrigger(Constants.AttackAnimatorParam);
        }

        private void HandleBubbleStartAttack()
        {
            IsAttacking = true;
        }

        private void HandleBubbleCompleteAttack()
        {
            IsAttacking = false;
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

                healthCmp.TakeDamage(Damage);
                _hitTargets.Add(healthCmp);
            }
        }

        public void CancelAttack()
        {
            _animatorCmp.ResetTrigger(Constants.AttackAnimatorParam);
        }
    }
}