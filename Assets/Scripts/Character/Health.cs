using System;
using System.Collections;
using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RPG.Character
{
    public class Health : MonoBehaviour
    {
        private Animator _animatorCmp;
        private bool _isDefeated;
        private BubbleEvent _bubbleEventCmp;
        private Renderer[] _renderers;
        private bool _isInvincible;
        private Coroutine _invincibilityCoroutine;

        [SerializeField] private float healAmount = 15f;

        [Header("Invincibility Settings")]
        [Tooltip("How many times the character flickers during invincibility")]
        [SerializeField]
        private int flickerCount = 5;

        public int potionCount = 1;
        public event UnityAction OnStartDefeated = () => { };

        [NonSerialized] public float HealthPoints;
        [NonSerialized] public Slider SliderCmp;
        [NonSerialized] public float InvincibilityDuration = 1f;

        private void Awake()
        {
            _animatorCmp = GetComponentInChildren<Animator>();
            _bubbleEventCmp = GetComponentInChildren<BubbleEvent>();
            SliderCmp = GetComponentInChildren<Slider>();
            _renderers = GetComponentsInChildren<Renderer>();
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
            // Skip damage if invincible
            if (_isInvincible) return;

            // Apply defense reduction if defending
            var combatCmp = GetComponent<Combat>();
            if (combatCmp)
            {
                var defenseReduction = combatCmp.GetDefenseReduction();
                damageAmount *= (1f - defenseReduction);
            }

            HealthPoints = Mathf.Max(HealthPoints - damageAmount, 0);

            if (CompareTag(Constants.PlayerTag))
            {
                EventManager.RaiseChangePlayerHealth(HealthPoints);
            }

            if (SliderCmp)
            {
                SliderCmp.value = HealthPoints;
            }

            if (HealthPoints != 0)
            {
                StartInvincibility();
                return;
            }

            Defeated();
        }

        private void Defeated()
        {
            if (_isDefeated) return;

            if (CompareTag(Constants.EnemyTag) || CompareTag(Constants.FinalBossTag))
            {
                OnStartDefeated.Invoke();
            }

            _isDefeated = true;
            _animatorCmp.SetTrigger(Constants.DefeatedAnimatorParam);
        }

        private void HandleBubbleCompleteDefeat()
        {
            if (CompareTag(Constants.PlayerTag))
                EventManager.RaiseGameOver();

            if (CompareTag(Constants.FinalBossTag))
                EventManager.RaiseVictory();

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

        private void StartInvincibility()
        {
            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
            }

            _invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }

        private IEnumerator InvincibilityCoroutine()
        {
            _isInvincible = true;

            // Flicker effect - blink on/off
            var flickerInterval = InvincibilityDuration / (flickerCount * 2f);

            for (var i = 0; i < flickerCount; i++)
            {
                // Turn off renderers
                SetRenderersEnabled(false);
                yield return new WaitForSeconds(flickerInterval);

                // Turn on renderers
                SetRenderersEnabled(true);
                yield return new WaitForSeconds(flickerInterval);
            }

            _isInvincible = false;

            // Ensure all renderers are visible
            SetRenderersEnabled(true);
        }

        private void SetRenderersEnabled(bool isEnabled)
        {
            foreach (var rend in _renderers)
            {
                if (rend)
                {
                    rend.enabled = isEnabled;
                }
            }
        }
    }
}