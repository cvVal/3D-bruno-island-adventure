using UnityEngine;

namespace RPG.Character
{
    public class AIAttackState : AIBaseState
    {
        public override void EnterState(EnemyController enemy)
        {
            enemy.MovementCmp.StopMovingAgent();
        }

        public override void UpdateState(EnemyController enemy)
        {
            if (enemy.DistanceFromPlayer > enemy.attackRange)
            {
                enemy.SwitchState(enemy.ChaseState);
                return;
            }
            
            Debug.Log("Attack!");
        }
    }
}