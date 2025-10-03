using RPG.Character;
using UnityEngine;

namespace RPG.Core
{
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.OnPortalEnter += HandlePortalEnter;
        }
        
        private void OnDisable()
        {
            EventManager.OnPortalEnter -= HandlePortalEnter;
        }
        
        private void HandlePortalEnter(Collider player, int nextSceneIndex)
        {
            var playerControllerCmp = player.GetComponent<PlayerController>();
            
            PlayerPrefs.SetFloat("Health", playerControllerCmp.HealthCmp.HealthPoints);
            PlayerPrefs.SetInt("Potions", playerControllerCmp.HealthCmp.potionCount);
            PlayerPrefs.SetFloat("Damage", playerControllerCmp.CombatCmp.Damage);
            PlayerPrefs.SetInt("Weapon", (int)playerControllerCmp.weapon);
            PlayerPrefs.SetInt("SceneIndex", nextSceneIndex);
        }
    }
}