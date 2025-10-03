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
            if (!enemy.Player)
            {
                enemy.CombatCmp.CancelAttack();
                return;
            }
            
            if (enemy.DistanceFromPlayer > enemy.attackRange)
            {
                enemy.CombatCmp.CancelAttack();
                enemy.SwitchState(enemy.ChaseState);
                return;
            }
            
            if (enemy.HasOpenedUI) return;

            enemy.CombatCmp.StartAttack();
            enemy.transform.LookAt(enemy.Player.transform);
        }
    }
}