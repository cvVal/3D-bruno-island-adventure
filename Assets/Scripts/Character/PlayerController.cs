using RPG.Core;
using RPG.Quest;
using RPG.Utility;
using UnityEngine;

namespace RPG.Character
{
    public class PlayerController : MonoBehaviour
    {
        private Health _healthCmp;
        private Combat _combatCmp;
        private GameObject _axeWeapon;
        private GameObject _swordWeapon;

        public CharacterStatsSo stats;
        public Weapons weapon = Weapons.Axe;

        private void Awake()
        {
            if (!stats) Debug.LogWarning("${name} does not have stats assigned.", this);

            _healthCmp = GetComponent<Health>();
            _combatCmp = GetComponent<Combat>();
            _axeWeapon = GameObject.FindGameObjectWithTag(Constants.AxeTag);
            _swordWeapon = GameObject.FindGameObjectWithTag(Constants.SwordTag);
        }
        
        private void OnEnable()
        {
            EventManager.OnReward += HandleReward;
        }
        
        private void OnDisable()
        {
            EventManager.OnReward -= HandleReward;
        }

        private void Start()
        {
            _healthCmp.HealthPoints = stats.health;
            _combatCmp.Damage = stats.damage;
            
            EventManager.RaiseChangePlayerHealth(_healthCmp.HealthPoints);
            SetWeapon();
        }

        private void HandleReward(RewardSo rewardSo)
        {
            _healthCmp.HealthPoints += rewardSo.bonusHealth;
            _healthCmp.potionCount += rewardSo.bonusPotion;
            _combatCmp.Damage += rewardSo.bonusDamage;
            
            EventManager.RaiseChangePlayerHealth(_healthCmp.HealthPoints);
            EventManager.RaiseChangePlayerPotions(_healthCmp.potionCount);

            if (!rewardSo.forceWeaponSwap) return;
            
            weapon = rewardSo.weapon;
            SetWeapon();
        }

        private void SetWeapon()
        {
            if (weapon == Weapons.Axe)
            {
                _axeWeapon.SetActive(true);
                _swordWeapon.SetActive(false);
            }
            else
            {
                _swordWeapon.SetActive(true);
                _axeWeapon.SetActive(false);
            }
        }
    }
}