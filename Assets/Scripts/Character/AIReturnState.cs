using UnityEngine;

namespace RPG.Character
{
    public class AIReturnState : AIBaseState
    {
        public override void EnterState(EnemyController enemy)
        {
            Debug.Log("Entering Return State");
            enemy.MovementCmp.MoveAgentByDestination(enemy.OriginalPosition);
        }

        public override void UpdateState(EnemyController enemy)
        {
            if (enemy.DistanceFromPlayer < enemy.chaseRange)
            {
                enemy.SwitchState(enemy.ChaseState);
                return;
            }
        }
    }
}
