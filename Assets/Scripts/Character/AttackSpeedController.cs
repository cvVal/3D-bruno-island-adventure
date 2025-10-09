using RPG.Utility;
using UnityEngine;

namespace RPG.Character
{
    /// <summary>
    /// Attach this to the Attack animation state in the Animator Controller.
    /// It will automatically set the animation speed based on the attackSpeed parameter.
    /// </summary>
    public class AttackSpeedController : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var attackSpeed = animator.GetFloat(Constants.AttackSpeedAnimatorParam);
            animator.speed = attackSpeed;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset speed to normal when exiting attack state
            animator.speed = 1f;
        }
    }
}

