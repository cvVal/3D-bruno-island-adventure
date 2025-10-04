using RPG.Utility;
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

        [Tooltip("Add all Cinemachine cameras used in this cutscene. They will be disabled when not playing.")]
        [SerializeField] private CinemachineCamera[] cinematicCameras;
        [SerializeField] private bool disableCamerasWhenNotPlaying = true;

        private void Awake()
        {
            _playableDirectorCmp = GetComponent<PlayableDirector>();
            _colliderCmp = GetComponent<Collider>();
            _cutsceneCameras = cinematicCameras;
        }

        private void Start()
        {
            _colliderCmp.enabled = !PlayerPrefs.HasKey(Constants.PlayerPrefsSceneIndex);

            if (disableCamerasWhenNotPlaying)
            {
                ToggleCutsceneCameras(false);
            }
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