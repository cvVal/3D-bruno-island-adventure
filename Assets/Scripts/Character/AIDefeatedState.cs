namespace RPG.Character
{
    public class AIDefeatedState : AIBaseState
    {

        public override void EnterState(EnemyController enemy)
        {
            if (enemy.AudioSourceCmp && enemy.deathClip)
                enemy.AudioSourceCmp.PlayOneShot(enemy.deathClip);
        }

        public override void UpdateState(EnemyController enemy) { }
    }
}