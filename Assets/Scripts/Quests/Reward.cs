using RPG.Core;
using UnityEngine;

namespace RPG.Quest
{
    public class Reward : MonoBehaviour
    {
        private bool _rewardTaken;
        
        [SerializeField] private RewardSo rewardSo;

        public void SendReward()
        {
            if (_rewardTaken) return;
            
            _rewardTaken = true;
            
            EventManager.RaiseReward(rewardSo);
        }
    }
}