using UnityEngine;

namespace RPG.Character
{
    public class AIReturnState : AIBaseState
    {
        private Vector3 _targetPosition;
        
        public override void EnterState(EnemyController enemy)
        {
            enemy.MovementCmp.UpdateAgentSpeed(enemy.stats.walkSpeed);
            
            if (enemy.PatrolCmp is not null)
            {
                _targetPosition = enemy.PatrolCmp.GetNextPosition();
                enemy.MovementCmp.MoveAgentByDestination(_targetPosition);
            }
            else
            {
                enemy.MovementCmp.MoveAgentByDestination(enemy.OriginalPosition);
            }
        }

        public override void UpdateState(EnemyController enemy)
        {
            if (enemy.DistanceFromPlayer < enemy.chaseRange)
            {
                enemy.SwitchState(enemy.ChaseState);
                return;
            }

            if (enemy.MovementCmp.HasReachedDestination())
            {
                if (enemy.PatrolCmp is not null)
                {
                    enemy.SwitchState(enemy.PatrolState);
                    return;
                }
                
                // For non patrolling enemies, just stop moving and face original direction
                enemy.MovementCmp.Rotate(enemy.MovementCmp.OriginalForwardVector);
            }
            else
            {
                if (enemy.PatrolCmp is not null)
                {
                    var newForwardVector = _targetPosition - enemy.transform.position;
                    newForwardVector.y = 0;
                    
                    enemy.MovementCmp.Rotate(newForwardVector);
                }
                else
                {
                    var newForwardVector = enemy.OriginalPosition - enemy.transform.position;
                    newForwardVector.y = 0;
                    
                    enemy.MovementCmp.Rotate(newForwardVector);
                }
            }
        }
    }
}