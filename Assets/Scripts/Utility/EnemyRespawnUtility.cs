using System;
using UnityEngine;

namespace RPG.Utility
{
    /// <summary>
    /// Utility class for handling enemy respawn logic, including tracking defeated enemies
    /// and determining if they should respawn.
    /// </summary>
    public static class EnemyRespawnUtility
    {
        // The time in seconds before an enemy respawns (3 minutes).
        private const int RespawnTimeInSeconds = 180;

        // Prefix used for storing enemy defeat data in PlayerPrefs.
        private const string KeyPrefix = "EnemyDefeat_";

        /// <summary>
        /// Generates a unique enemy ID based on the scene name and the enemy's world position.
        /// This ensures that enemies are uniquely identified across different scenes.
        /// </summary>
        /// <param name="sceneName">The name of the scene where the enemy exists.</param>
        /// <param name="position">The world position of the enemy.</param>
        /// <returns>A unique string identifier for the enemy.</returns>
        public static string GenerateEnemyID(string sceneName, Vector3 position)
        {
            return $"{sceneName}_{position.x:F2}_{position.y:F2}_{position.z:F2}";
        }

        /// <summary>
        /// Saves the defeated state of an enemy by storing the current timestamp in PlayerPrefs.
        /// If the enemy is already marked as defeated, the timestamp is not updated.
        /// </summary>
        /// <param name="enemyId">The unique identifier of the enemy.</param>
        public static void SaveDefeatedEnemy(string enemyId)
        {
            var key = KeyPrefix + enemyId;

            // Check if the enemy is already marked as defeated.
            if (PlayerPrefs.HasKey(key))
            {
                Debug.Log($"Enemy {enemyId} already defeated and waiting to respawn.");
                return;
            }

            // Save the current timestamp as the defeat time.
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            PlayerPrefs.SetString(key, timestamp.ToString());
            PlayerPrefs.Save();

            Debug.Log($"Enemy {enemyId} defeated at {DateTime.Now:HH:mm:ss}. Will respawn in {RespawnTimeInSeconds}s");
        }

        /// <summary>
        /// Determines if an enemy should remain in the defeated state based on the respawn timer.
        /// If the respawn time has elapsed, the enemy is allowed to respawn, and the defeat data is cleared.
        /// </summary>
        /// <param name="enemyId">The unique identifier of the enemy.</param>
        /// <returns>
        /// True if the enemy should remain defeated (respawn time has not elapsed),
        /// false if the enemy can respawn.
        /// </returns>
        public static bool ShouldEnemyBeDefeated(string enemyId)
        {
            var key = KeyPrefix + enemyId;

            // If no defeat data exists for the enemy, it is not defeated.
            if (!PlayerPrefs.HasKey(key)) return false;

            var timestampString = PlayerPrefs.GetString(key);
            if (!long.TryParse(timestampString, out var defeatTimestamp)) return false;

            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeSinceDefeat = currentTime - defeatTimestamp;

            // If the respawn time has passed, clear the defeat data and allow respawn.
            if (timeSinceDefeat < RespawnTimeInSeconds) return true;

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"Enemy {enemyId} respawned after {timeSinceDefeat}s");

            return false;
        }
    }
}