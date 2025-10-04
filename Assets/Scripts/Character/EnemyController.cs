using System;
using RPG.Core;
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
        [NonSerialized] public bool HasOpenedUI;
        [NonSerialized] public string EnemyID;

        private void Awake()
        {
            if (!stats) Debug.LogWarning("${name} does not have stats assigned.", this);

            // Generate a persistent ID based on scene and position
            var sceneName = gameObject.scene.name;
            var pos = transform.position;
            EnemyID = $"{sceneName}_{pos.x:F2}_{pos.y:F2}_{pos.z:F2}";

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
            EventManager.OnToggleUI += HandleToggleUI;
        }

        private void OnDisable()
        {
            _healthCmp.OnStartDefeated -= HandleStartDefeated;
            EventManager.OnToggleUI -= HandleToggleUI;
        }

        private void Start()
        {
            // Check if this enemy was already defeated and hasn't respawned yet
            if (EnemyRespawnUtility.ShouldEnemyBeDefeated(EnemyID))
            {
                Destroy(gameObject);
                return;
            }

            _currentState.EnterState(this);

            _healthCmp.HealthPoints = stats.health;
            CombatCmp.Damage = stats.damage;

            if (!_healthCmp.SliderCmp) return;
            _healthCmp.SliderCmp.maxValue = stats.health;
            _healthCmp.SliderCmp.value = stats.health;
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

        private void HandleToggleUI(bool isOpened)
        {
            HasOpenedUI = isOpened;
        }
    }
}