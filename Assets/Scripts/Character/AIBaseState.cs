namespace RPG.Character
{
    /// <summary>
    /// Base class for AI state machine states that control enemy behavior.
    /// Implements the State pattern for managing different AI behaviors (e.g., patrol, chase, attack).
    /// </summary>
    public abstract class AIBaseState
    {
        /// <summary>
        /// Called when the AI transitions into this state.
        /// Use this method to initialize state-specific behavior and reset any necessary variables.
        /// </summary>
        /// <param name="enemy">The enemy controller that is entering this state.</param>
        public abstract void EnterState(EnemyController enemy);
        
        /// <summary>
        /// Called every frame while the AI is in this state.
        /// Use this method to implement the state's ongoing behavior and check for state transition conditions.
        /// </summary>
        /// <param name="enemy">The enemy controller that is currently in this state.</param>
        public abstract void UpdateState(EnemyController enemy);
    }
}