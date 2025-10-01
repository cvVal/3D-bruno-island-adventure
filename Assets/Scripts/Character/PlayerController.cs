using UnityEngine;

namespace RPG.Character
{
    public class PlayerController : MonoBehaviour
    {
        private Health _healthCmp;
        private Combat _combatCmp;

        public CharacterStatsSo stats;

        private void Awake()
        {
            if (!stats) Debug.LogWarning("${name} does not have stats assigned.", this);

            _healthCmp = GetComponent<Health>();
            _combatCmp = GetComponent<Combat>();
        }

        private void Start()
        {
            _healthCmp.HealthPoints = stats.health;
            _combatCmp.Damage = stats.damage;
        }
    }
}