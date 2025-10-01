using System;
using RPG.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Character
{
    public class Health : MonoBehaviour
    {
        private Animator _animatorCmp;
        private bool _isDefeated;
        private BubbleEvent _bubbleEventCmp;

        public event UnityAction OnStartDefeated = () => { };
        
        [NonSerialized] public float HealthPoints;

        private void Awake()
        {
            _animatorCmp = GetComponentInChildren<Animator>();
            _bubbleEventCmp = GetComponentInChildren<BubbleEvent>();
        }

        private void OnEnable()
        {
            _bubbleEventCmp.OnBubbleCompleteDefeat += HandleBubbleCompleteDefeat;
        }

        private void OnDisable()
        {
            _bubbleEventCmp.OnBubbleCompleteDefeat -= HandleBubbleCompleteDefeat;
        }

        public void TakeDamage(float damageAmount)
        {
            HealthPoints = Mathf.Max(HealthPoints - damageAmount, 0);

            if (HealthPoints != 0) return;

            Defeated();
        }

        private void Defeated()
        {
            if (_isDefeated) return;

            if (CompareTag(Constants.EnemyTag))
            {
                OnStartDefeated.Invoke();
            }

            _isDefeated = true;
            _animatorCmp.SetTrigger(Constants.DefeatedAnimatorParam);
        }

        private void HandleBubbleCompleteDefeat()
        {
            Destroy(gameObject);
        }
    }
}