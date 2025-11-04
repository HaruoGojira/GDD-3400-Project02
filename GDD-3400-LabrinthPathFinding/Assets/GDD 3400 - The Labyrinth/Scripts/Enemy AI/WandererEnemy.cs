using UnityEngine;


namespace GDD3400.Labyrinth
{
    public class WandererEnemy : EnemyAgent
    {
        //movement variables
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _wanderSpeed = 2.0f;
        [SerializeField] private float _chaseSpeed = 4.0f;
        [SerializeField] private float _detectionRange = 10.0f;
        [SerializeField] private float _wanderRadius = 5.0f;
        [SerializeField] private float _chaseTimer = 5.0f;


        //Enum for WandererEnemy states
        private enum WandererState
        {
            Wandering,
            Chasing
        }
        private WandererState _currentState;


        /// <summary>
        /// This method is called when the script instance is being loaded
        /// </summary>
        public void Awake()
        {
            //inherits the Rigidbody from EnemyAgent otherwise it would override it
            base.Awake();

            //find player transform
            if (_playerTransform == null)
            {
                GameObject playerObject = GameObject.FindWithTag("Player");
                if (playerObject != null)
                {
                    _playerTransform = playerObject.transform;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Update perception and decision making each frame
            Perception();
            DecisionMaking();
        }

        /// <summary>
        /// FixedUpdate is called at a fixed interval and is independent of frame rate
        /// </summary>
        void FixedUpdate()
        {
            // Handle movement based on current state
            switch (_currentState)
            {
                case WandererState.Wandering:
                    // Move randomly within wander radius
                    Vector3 wanderDirection = Random.insideUnitSphere * _wanderRadius;
                    wanderDirection.y = 0;
                    _rb.MovePosition(transform.position + wanderDirection.normalized * _wanderSpeed);
                    break;
                case WandererState.Chasing:
                    // Move towards the player
                    Vector3 chaseDirection = (_playerTransform.position - transform.position).normalized;
                    _rb.MovePosition(transform.position + chaseDirection * _chaseSpeed);
                    break;
            }
        }

        //helps with enemy perception
        void Perception()
        {
            if (_playerTransform == null) return;

            //if the player is within detection range, start chasing
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= _detectionRange)
            {
                //reset chase timer and switch to chasing state
                _currentState = WandererState.Chasing;
                _chaseTimer = 5f; 
            }
            //else, continue wandering
            else
            {
                _currentState = WandererState.Wandering;
            }
        }

        //helps with decision making
        void DecisionMaking()
        {
            switch (_currentState)
            {
                case WandererState.Wandering:
                    Wander();
                    break;
                case WandererState.Chasing:
                    ChasePlayer();
                    break;
            }
        }

        /// <summary>
        /// handles wandering behavior
        /// </summary>
        void Wander()
        {
            //sets up the wandering state movements
            Vector3 randomDir = Random.insideUnitSphere * _wanderRadius;
            randomDir.y = 0;
            Vector3 wanderTarget = transform.position + randomDir;
        }

        /// <summary>
        /// handles chasing the player
        /// </summary>
        void ChasePlayer()
        {
            _currentState = WandererState.Wandering;
            _chaseTimer -= Time.deltaTime;

            //if timer runs out, go back to wandering
            if (_chaseTimer <= 0f)
            {
                _currentState = WandererState.Wandering;
            }
        }

        /// <summary>
        /// Visualizes detection range and wander radius in the editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw detection range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
            // Draw wander radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _wanderRadius);
        }

        /// <summary>
        /// funtion to move to a target position
        /// </summary>
        /// <param name="targetPosition"></param>
        private void MoveToTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            _rb.MovePosition(transform.position + direction * _wanderSpeed * Time.fixedDeltaTime);
        }

    }


}



///Pseudo Code for WandererEnemy:
///What I wanted for this enemy was a simple enemy that would wander around randomly until the player got close enough, then it would chase the player for a set amount of time before returning to wandering.
///
