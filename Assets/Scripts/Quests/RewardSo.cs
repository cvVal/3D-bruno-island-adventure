using RPG.Character;
using UnityEngine;

namespace RPG.Quest
{
    [
        CreateAssetMenu(
            fileName = "Reward",
            menuName = "RPG/Reward",
            order = 2
        )
    ]
    public class RewardSo : ScriptableObject
    {
        public float bonusHealth;
        public float bonusDamage;
        public int bonusPotion;
        public float bonusAttackSpeed;
        public bool forceWeaponSwap;
        public Weapons weapon = Weapons.Sword;
    }
}