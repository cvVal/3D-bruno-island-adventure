using System;
using RPG.Core;
using RPG.Quest;
using RPG.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character
{
    public class PlayerController : MonoBehaviour
    {
        private GameObject _axeWeapon;
        private GameObject _swordWeapon;

        public CharacterStatsSo stats;
        public Weapons weapon = Weapons.Axe;
        private Movement _movementCmp;

        [NonSerialized] public Health HealthCmp;
        [NonSerialized] public Combat CombatCmp;

        private void Awake()
        {
            if (!stats) Debug.LogWarning("${name} does not have stats assigned.", this);

            HealthCmp = GetComponent<Health>();
            CombatCmp = GetComponent<Combat>();
            _movementCmp = GetComponent<Movement>();
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
            if (PlayerPrefs.HasKey(Constants.PlayerPrefsHealth))
            {
                HealthCmp.HealthPoints = PlayerPrefs.GetFloat(Constants.PlayerPrefsHealth);
                HealthCmp.potionCount = PlayerPrefs.GetInt(Constants.PlayerPrefsPotions);
                CombatCmp.Damage = PlayerPrefs.GetFloat(Constants.PlayerPrefsDamage);
                CombatCmp.AttackSpeed = PlayerPrefs.GetFloat(Constants.PlayerPrefsAttackSpeed);
                weapon = (Weapons)PlayerPrefs.GetInt(Constants.PlayerPrefsWeapon);

                var agentCmp = GetComponent<NavMeshAgent>();
                var portalCmp = FindFirstObjectByType<Portal>();

                agentCmp.Warp(portalCmp.spawnPoint.position);
                transform.rotation = portalCmp.spawnPoint.rotation;
            }
            else
            {
                HealthCmp.HealthPoints = stats.health;
                CombatCmp.Damage = stats.damage;
                CombatCmp.AttackSpeed = stats.attackSpeed;
            }

            // Initialize dash stats from CharacterStatsSo
            _movementCmp.DashDistance = stats.dashDistance;
            _movementCmp.DashDuration = stats.dashDuration;
            _movementCmp.DashCooldown = stats.dashCooldown;
            
            // Initialize recoil stats from CharacterStatsSo
            _movementCmp.RecoilDistance = stats.recoilDistance;
            _movementCmp.RecoilDuration = stats.recoilDuration;
            
            // Initialize invincibility duration from CharacterStatsSo
            HealthCmp.InvincibilityDuration = stats.invincibilityDuration;

            EventManager.RaiseChangePlayerHealth(HealthCmp.HealthPoints);
            SetWeapon();
        }

        private void HandleReward(RewardSo rewardSo)
        {
            HealthCmp.HealthPoints += rewardSo.bonusHealth;
            HealthCmp.potionCount += rewardSo.bonusPotion;
            CombatCmp.Damage += rewardSo.bonusDamage;
            CombatCmp.AttackSpeed += rewardSo.bonusAttackSpeed;

            EventManager.RaiseChangePlayerHealth(HealthCmp.HealthPoints);
            EventManager.RaiseChangePlayerPotions(HealthCmp.potionCount);

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