using System;
using RPG.Utility;
using UnityEngine;

namespace RPG.Character
{
    public class EnemyController : MonoBehaviour
    {
        private AIBaseState _currentState;
        private Health _healthCmp;

        public readonly AIReturnState ReturnState = new();
        public readonly AIChaseState ChaseState = new();
        public readonly AIAttackState AttackState = new();
        public readonly AIPatrolState PatrolState = new();
        public readonly AIDefeatedState DefeatedState = new();

        public CharacterStatsSo stats;
        public float chaseRange = 2.5f;
        public float attackRange = 0.75f;

        [NonSerialized] public GameObject Player;
        [NonSerialized] public Movement MovementCmp; // Cmp = Component
        [NonSerialized] public float DistanceFromPlayer;
        [NonSerialized] public Vector3 OriginalPosition;
        [NonSerialized] public Patrol PatrolCmp;
        [NonSerialized] public Combat CombatCmp;
        
        private void Awake()
        {
            if (!stats) Debug.LogWarning("${name} does not have stats assigned.", this);
            
            _currentState = ReturnState;
            
            Player = GameObject.FindWithTag(Constants.PlayerTag);
            MovementCmp = GetComponent<Movement>();
            PatrolCmp = GetComponent<Patrol>();
            _healthCmp = GetComponent<Health>();
            CombatCmp = GetComponent<Combat>();
            
            OriginalPosition = transform.position;
        }

        private void OnEnable()
        {
            _healthCmp.OnStartDefeated += HandleStartDefeated;
        }
        
        private void OnDisable()
        {
            _healthCmp.OnStartDefeated -= HandleStartDefeated;
        }

        private void Start()
        {
            _currentState.EnterState(this);
            
            _healthCmp.HealthPoints = stats.health;
            CombatCmp.Damage = stats.damage;
        }

        private void Update()
        {
            CalculateDistanceFromPlayer();
            _currentState.UpdateState(this);
        }
        
        public void SwitchState(AIBaseState newState)
        {
            _currentState = newState;
            _currentState.EnterState(this);
        }
        
        private void CalculateDistanceFromPlayer()
        {
            if (!Player) return;
            
            var enemyPosition = transform.position;
            var playerPosition = Player.transform.position;
            DistanceFromPlayer = Vector3.Distance(enemyPosition, playerPosition);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
        
        private void HandleStartDefeated()
        {
            SwitchState(DefeatedState);
            _currentState.EnterState(this);
        }
    }
}
