using RPG.Character;
using RPG.Utility;
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
            
            PlayerPrefs.SetFloat(Constants.PlayerPrefsHealth, playerControllerCmp.HealthCmp.HealthPoints);
            PlayerPrefs.SetInt(Constants.PlayerPrefsPotions, playerControllerCmp.HealthCmp.potionCount);
            PlayerPrefs.SetFloat(Constants.PlayerPrefsDamage, playerControllerCmp.CombatCmp.Damage);
            PlayerPrefs.SetInt(Constants.PlayerPrefsWeapon, (int)playerControllerCmp.weapon);
            PlayerPrefs.SetInt(Constants.PlayerPrefsSceneIndex, nextSceneIndex);
        }
    }
}