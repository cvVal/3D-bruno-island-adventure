using System;
using UnityEngine;

namespace RPG.Utility
{
    public static class EnemyRespawnUtility
    {
        // 3 minutes
        private const int RespawnTimeInSeconds = 180;
        private const string KeyPrefix = "EnemyDefeat_";

        public static void SaveDefeatedEnemy(string enemyId)
        {
            var key = KeyPrefix + enemyId;

            // Check if enemy is already in defeated state - if so, don't update timestamp
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log($"Enemy {enemyId} already defeated and waiting to respawn.");
                return;
            }

            // Save current timestamp
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            PlayerPrefs.SetString(key, timestamp.ToString());
            PlayerPrefs.Save();

            Debug.Log($"Enemy {enemyId} defeated at {DateTime.Now:HH:mm:ss}. Will respawn in {RespawnTimeInSeconds}s");
        }

        public static bool ShouldEnemyBeDefeated(string enemyId)
        {
            var key = KeyPrefix + enemyId;

            if (!PlayerPrefs.HasKey(key)) return false;

            var timestampString = PlayerPrefs.GetString(key);
            if (!long.TryParse(timestampString, out var defeatTimestamp)) return false;

            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeSinceDefeat = currentTime - defeatTimestamp;

            // If respawn time has passed, remove entry and allow respawn
            if (timeSinceDefeat < RespawnTimeInSeconds) return true;

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"Enemy {enemyId} respawned after {timeSinceDefeat}s");

            return false;
        }
    }
}