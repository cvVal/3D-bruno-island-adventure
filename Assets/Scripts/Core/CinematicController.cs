using RPG.Utility;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Core
{
    public class CinematicController : MonoBehaviour
    {
        private PlayableDirector _playableDirectorCmp;
        private Collider _colliderCmp;

        private void Awake()
        {
            _playableDirectorCmp = GetComponent<PlayableDirector>();
            _colliderCmp = GetComponent<Collider>();
        }

        private void Start()
        {
            _colliderCmp.enabled = !PlayerPrefs.HasKey(Constants.PlayerPrefsSceneIndex);
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
            EventManager.RaiseCutSceneUpdated(false);
        }

        private void HandleStoppedEvent(PlayableDirector director)
        {
            EventManager.RaiseCutSceneUpdated(true);
        }
    }
}