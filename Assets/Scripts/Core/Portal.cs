using RPG.Utility;
using UnityEngine;

namespace RPG.Core
{
    public class Portal : MonoBehaviour
    {
        private Collider _colliderCmp;

        [SerializeField] private int nextSceneIndex;

        public Transform spawnPoint;

        private void Awake()
        {
            _colliderCmp = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Constants.PlayerTag)) return;

            _colliderCmp.enabled = false;

            EventManager.RaisePortalEnter(other, nextSceneIndex);
            
            var audioSource = GameObject
                .FindGameObjectWithTag(Constants.GameManagerTag)
                .GetComponent<AudioSource>();
            
            StartCoroutine(
                SceneTransition.Initiate(nextSceneIndex, audioSource)
            );
        }
    }
}