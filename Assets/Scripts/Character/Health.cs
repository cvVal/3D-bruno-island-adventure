using System;
using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace RPG.Character
{
    public class Health : MonoBehaviour
    {
        private Animator _animatorCmp;
        private bool _isDefeated;
        private BubbleEvent _bubbleEventCmp;

        [SerializeField] private int potionCount = 1;
        [SerializeField] private float healAmount = 15f;
        
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

        private void Start()
        {
            if (CompareTag(Constants.PlayerTag))
            {
                EventManager.RaiseChangePlayerPotions(potionCount);
            }
        }

        public void TakeDamage(float damageAmount)
        {
            HealthPoints = Mathf.Max(HealthPoints - damageAmount, 0);
            
            if (CompareTag(Constants.PlayerTag))
            {
                EventManager.RaiseChangePlayerHealth(HealthPoints);
            }

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

        public void HandleHeal(InputAction.CallbackContext context)
        {
            if (!context.performed || potionCount <= 0) return;
            
            potionCount--;
            HealthPoints += healAmount;
            
            EventManager.RaiseChangePlayerHealth(HealthPoints);
            EventManager.RaiseChangePlayerPotions(potionCount);
        }
    }
}