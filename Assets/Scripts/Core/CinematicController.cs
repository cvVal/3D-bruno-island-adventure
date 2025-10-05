using RPG.Utility;
using RPG.Character;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Core
{
    public class CinematicController : MonoBehaviour
    {
        private PlayableDirector _playableDirectorCmp;
        private Collider _colliderCmp;
        private CinemachineCamera[] _cutsceneCameras;
        private string _enemyID;

        [Tooltip("Add all Cinemachine cameras used in this cutscene. They will be disabled when not playing.")]
        [SerializeField]
        private CinemachineCamera[] cinematicCameras;

        [Tooltip("Optional: Reference to enemy that must be alive for cutscene to play")] 
        [SerializeField]
        private EnemyController associatedEnemy;

        [SerializeField] private bool disableCamerasWhenNotPlaying = true;
        [SerializeField] private bool customPlayOnAwake;

        private void Awake()
        {
            _playableDirectorCmp = GetComponent<PlayableDirector>();
            _colliderCmp = GetComponent<Collider>();
            _cutsceneCameras = cinematicCameras;

            if (!associatedEnemy) return;

            // Generate enemy ID using centralized utility function
            _enemyID = EnemyRespawnUtility.GenerateEnemyID(
                gameObject.scene.name, 
                associatedEnemy.transform.position
            );
        }

        private void Start()
        {
            _colliderCmp.enabled = !PlayerPrefs.HasKey(Constants.PlayerPrefsSceneIndex);

            if (disableCamerasWhenNotPlaying)
            {
                ToggleCutsceneCameras(false);
            }

            if (!customPlayOnAwake) return;

            // Check if enemy is alive before playing
            if (!ShouldPlayCutscene()) return;

            _colliderCmp.enabled = false;
            _playableDirectorCmp.Play();
        }

        private void OnEnable()
        {
            _playableDirectorCmp.played += HandlePlayedEvent;
            _playableDirectorCmp.stopped += HandleStoppedEvent;
        }

        private void OnDisable()
        {
            _playableDirectorCmp.played -= HandlePlayedEvent;
            _playableDirectorCmp.stopped -= HandleStoppedEvent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Constants.PlayerTag)) return;

            // Check if enemy is alive before playing
            if (!ShouldPlayCutscene()) return;

            _playableDirectorCmp.Play();

            _colliderCmp.enabled = false;
        }

        private void HandlePlayedEvent(PlayableDirector director)
        {
            if (disableCamerasWhenNotPlaying)
            {
                ToggleCutsceneCameras(true);
            }

            EventManager.RaiseCutSceneUpdated(false);
        }

        private void HandleStoppedEvent(PlayableDirector director)
        {
            if (disableCamerasWhenNotPlaying)
            {
                ToggleCutsceneCameras(false);
            }

            EventManager.RaiseCutSceneUpdated(true);
        }

        private bool ShouldPlayCutscene()
        {
            // If no enemy is associated, always play
            if (!associatedEnemy) return true;

            // Check if the enemy is defeated and hasn't respawned yet
            if (!EnemyRespawnUtility.ShouldEnemyBeDefeated(_enemyID)) return true;

            Debug.Log($"Cutscene skipped - enemy {_enemyID} is defeated and hasn't respawned yet.");
            return false;
        }

        private void ToggleCutsceneCameras(bool isEnabled)
        {
            if (_cutsceneCameras == null || _cutsceneCameras.Length == 0) return;

            foreach (var cinemachineCamera in _cutsceneCameras)
            {
                if (cinemachineCamera)
                {
                    cinemachineCamera.enabled = isEnabled;
                }
            }
        }
    }
}