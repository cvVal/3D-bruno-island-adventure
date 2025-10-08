using RPG.Core;
using UnityEngine;

namespace RPG.Quest
{
    public class Reward : MonoBehaviour
    {
        private bool _rewardTaken;
        private AudioSource _audioSourceCmp;

        [SerializeField] private RewardSo rewardSo;

        [Header("Audio Settings")] public AudioClip rewardClip;

        private void Awake()
        {
            _audioSourceCmp = GetComponent<AudioSource>();
        }

        public void SendReward()
        {
            if (_rewardTaken) return;

            _rewardTaken = true;

            if (!rewardSo)
            {
                Debug.LogWarning(
                    $"[{gameObject.name}] Reward component has no RewardSo assigned! Please assign a reward in the Inspector."
                );
                return;
            }

            EventManager.RaiseReward(rewardSo);

            if (rewardClip)
                _audioSourceCmp.PlayOneShot(rewardClip);
        }
    }
}