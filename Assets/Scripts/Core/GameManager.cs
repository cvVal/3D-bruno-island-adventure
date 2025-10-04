using System.Collections.Generic;
using RPG.Character;
using RPG.Utility;
using UnityEngine;

namespace RPG.Core
{
    public class GameManager : MonoBehaviour
    {
        private readonly List<string> _sceneEnemyIDs = new();
        private readonly List<GameObject> _enemiesAlive = new();

        private void Start()
        {
            var sceneEnemies = new List<GameObject>();

            sceneEnemies.AddRange(
                GameObject.FindGameObjectsWithTag(Constants.EnemyTag)
            );

            sceneEnemies.ForEach(sceneEnemy =>
                {
                    var enemyControllerCmp = sceneEnemy.GetComponent<EnemyController>();
                    _sceneEnemyIDs.Add(enemyControllerCmp.EnemyID);
                }
            );

            Debug.Log("Scene Enemy IDs: " + string.Join(", ", _sceneEnemyIDs));
        }

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

            _enemiesAlive.AddRange(
                GameObject.FindGameObjectsWithTag(Constants.EnemyTag)
            );

            _sceneEnemyIDs.ForEach(SaveDefeatedEnemies);
        }

        private void SaveDefeatedEnemies(string enemyId)
        {
            var isAlive = false;

            _enemiesAlive.ForEach(enemy =>
                {
                    var enemyID = enemy.GetComponent<EnemyController>().EnemyID;

                    if (enemyID == enemyId) isAlive = true;
                }
            );

            if (isAlive) return;

            // Save defeated enemy with timestamp for respawn timer
            EnemyRespawnUtility.SaveDefeatedEnemy(enemyId);
        }
    }
}