namespace RPG.Character
{
    public class AIChaseState : AIBaseState
    {
        public override void EnterState(EnemyController enemy)
        {
            enemy.MovementCmp.UpdateAgentSpeed(enemy.stats.runSpeed);
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
            
            var playerDirection = enemy.Player.transform.position - enemy.transform.position;
            enemy.MovementCmp.Rotate(playerDirection);
        }
    }
}