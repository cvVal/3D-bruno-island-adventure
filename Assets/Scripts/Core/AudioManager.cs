using RPG.Utility;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// Manages audio operations such as music volume ducking during pause/unpause.
    /// </summary>
    public static class AudioManager
    {
        private static float _originalMusicVolume;

        /// <summary>
        /// The volume multiplier to apply when music is ducked (0.0 to 1.0).
        /// Default is 0.3 (30% of original volume).
        /// </summary>
        private const float DuckVolumeMultiplier = 0.3f;

        /// <summary>
        /// Reduces the background music volume from the GameManager.
        /// Stores the original volume for later restoration.
        /// </summary>
        public static void DuckMusic()
        {
            var gameManagerAudioSourceCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<AudioSource>();
            
            if (!gameManagerAudioSourceCmp) return;

            _originalMusicVolume = gameManagerAudioSourceCmp.volume;
            gameManagerAudioSourceCmp.volume = _originalMusicVolume * DuckVolumeMultiplier;
        }

        /// <summary>
        /// Restores the background music volume to its original level.
        /// </summary>
        public static void RestoreMusic()
        {
            var gameManagerAudioSourceCmp = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<AudioSource>();
            
            if (!gameManagerAudioSourceCmp) return;

            gameManagerAudioSourceCmp.volume = _originalMusicVolume;
        }
    }
}