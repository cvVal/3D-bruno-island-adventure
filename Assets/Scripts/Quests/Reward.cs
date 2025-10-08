using RPG.Core;
using UnityEngine;

namespace RPG.Quest
{
    public class Reward : MonoBehaviour
    {
        private AudioSource _audioSourceCmp;

        [SerializeField] private RewardSo rewardSo;

        [Header("Audio Settings")] public AudioClip rewardClip;

        public bool HasRewardBeenClaimed { get; private set; }

        private void Awake()
        {
            _audioSourceCmp = GetComponent<AudioSource>();
        }

        public void SendReward()
        {
            if (HasRewardBeenClaimed) return;

            if (!rewardSo)
            {
                Debug.LogWarning(
                    $"[{gameObject.name}] Reward component has no RewardSo assigned! Please assign a reward in the Inspector."
                );
                return;
            }

            HasRewardBeenClaimed = true;

            var previewHandled = EventManager.RaiseRewardPreview(
                rewardSo,
                GrantReward
            );

            if (!previewHandled)
            {
                GrantReward();
            }
        }

        private void GrantReward()
        {
            EventManager.RaiseReward(rewardSo);

            if (rewardClip)
                _audioSourceCmp.PlayOneShot(rewardClip);
        }
    }
}