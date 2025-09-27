using UnityEngine;

namespace RPG.Character
{
    public class AIChaseState : AIBaseState
    {
        public override void EnterState(EnemyController enemy)
        {
            Debug.Log("Entering Chase State");
        }

        public override void UpdateState(EnemyController enemy)
        {
            if (enemy.DistanceFromPlayer > enemy.chaseRange)
            {
                enemy.SwitchState(enemy.ReturnState);
                return;
            }

            if (enemy.DistanceFromPlayer <= enemy.attackRange)
            {
                enemy.SwitchState(enemy.AttackState);
                return;
            }

            enemy.MovementCmp.MoveAgentByDestination(enemy.Player.transform.position);
        }
    }
}
