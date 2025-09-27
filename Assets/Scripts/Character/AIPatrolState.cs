namespace RPG.Character
{
    public class AIPatrolState : AIBaseState
    {
        public override void EnterState(EnemyController enemy)
        {
            enemy.PatrolCmp.ResetTimers();
        }

        public override void UpdateState(EnemyController enemy)
        {
            if (enemy.DistanceFromPlayer < enemy.chaseRange)
            {
                enemy.SwitchState(enemy.ChaseState);
                return;
            }
            
            enemy.PatrolCmp.CalculateNextPosition();
            
            var currentPosition = enemy.transform.position;
            var newPosition = enemy.PatrolCmp.GetNextPosition();
            var offset = newPosition - currentPosition;
            
            enemy.MovementCmp.MoveAgentByOffset(offset);
        }
    }
}