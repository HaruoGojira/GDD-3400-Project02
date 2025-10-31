using UnityEngine;

namespace GDD3400.Labyrinth
{
    public class GhostEnemy : EnemyAgent
    {
        //important variables
        private Vector3 _searchPosition;
        private float _searchTimer;

        //movement variables
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _phaseDuration = 3f;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _detectionRange = 15f;

        //Enum for GhostEnemy states
        private enum GhostState
        {
            Phasing,
            Chasing,
            Wandering
        }
        private GhostState _currentState;

        /// <summary>
        /// This method is called when the script instance is being loaded
        /// </summary>
        public void Awake()
        {
            //inherits the Rigidbody from EnemyAgent otherwise it would override it
            base.Awake();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Update perception and decision making each frame
            Perception();
            DecisionMaking();
        }

        // FixedUpdate is called at a fixed interval and is independent of frame rate
        void FixedUpdate()
        {
            
        }

        //helps with enemy perception
        void Perception()
        {
            //if the ghost is close enough to the player, start chasing
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= _detectionRange)
            {
                _currentState = GhostState.Chasing;
            }
            else
            {
                _currentState = GhostState.Wandering;
            }
        }

        //helps with decision making
        void DecisionMaking()
        {
            if (_currentState == GhostState.Phasing)
            {
                PhaseThroughWalls();
            }
            else if (_currentState == GhostState.Chasing)
            {
                ChasePlayer();
            }
            else if (_currentState == GhostState.Wandering)
            {
                Wander();
            }
        }

        //allows the ghost to phase through walls
        void PhaseThroughWalls()
        {
            // Implementation for phasing through walls
            if (_currentState != GhostState.Phasing)
            {
                _currentState = GhostState.Phasing;
                // Start phasing logic here

            }
        }

        //handles chasing the player
        void ChasePlayer()
        {

        }

        //handles wandering behavior
        void Wander()
        {

        }
    }
}
