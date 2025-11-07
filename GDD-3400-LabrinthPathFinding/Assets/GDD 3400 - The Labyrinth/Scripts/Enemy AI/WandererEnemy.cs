using UnityEngine;
using UnityEngine.AI;


namespace GDD3400.Labyrinth
{
    public class WandererEnemy : EnemyAgent
    {
        //movement variables
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _wanderSpeed = 2f;
        [SerializeField] private float _chaseSpeed = 4f;
        [SerializeField] private float _detectionRange = 15f;
        [SerializeField] private float _wanderRadius = 5f;
        [SerializeField] private float _chaseTimer = 5f;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private Vector3 _currentVelocity;
        [SerializeField] private float _distanceToThreshold = 1f;
        private float _chaseTimeRemaining;
        private Vector3 _lookDirection;

        //Path Node variables
        private PathNode _currentNode;
        private PathNode _targetNode;
        
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
            //inherits the Rigidbody from EnemyAgent
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }

            //initialize state
            _currentState = WandererState.Wandering;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Find the player in the scene
            _levelManager = FindAnyObjectByType<LevelManager>();

            // Initialize the current node based on the enemy's starting position
            _currentNode = _levelManager.GetNode(transform.position);
            PathNodeSelection();
        }

        // Update is called once per frame
        void Update()
        {
            //Update perception and decision making each frame
            Perception();
            DecisionMaking();
            gameObject.name = $"WandererEnemy - State: {_currentState}";
            Debug.Log(_chaseTimeRemaining);
        }

        /// <summary>
        /// FixedUpdate is called at a fixed interval and is independent of frame rate
        /// </summary>
        void FixedUpdate()
        {
            // If we have a floating target and we are not close enough to it, move towards it
            if (_floatingTarget != Vector3.zero && Vector3.Distance(transform.position, _floatingTarget) > _StoppingDistance)
            {
                // Calculate the direction to the target position
                Vector3 _direction = (_floatingTarget - transform.position).normalized;

                // Calculate the movement vector
                _currentVelocity = _direction * _currentSpeed;
            }

            // If we are close enough to the floating target, slow down
            else
            {
                _currentVelocity *= .95f;
            }

            // Calculate the desired rotation towards the movement vector
            if (_currentVelocity != Vector3.zero)
            {
                Quaternion _targetRotation = Quaternion.LookRotation(_currentVelocity);

                // Smoothly rotate towards the target rotation based on the turn rate
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _TurnRate);
            }

            _rb.linearVelocity = _currentVelocity;

        }

        #region Perception and Decision Making
        /// <summary>
        /// helps with enemy perception
        /// </summary>
        void Perception()
        {
            //checks the detection range to see if the player is close enough to chase
            if (_playerTransform == null) return;
            float _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // if the player is within detection range, switch to Chasing state
            if (_distanceToPlayer <= _detectionRange)
            {
                // this will only set the state to chasing if it is not already chasing
                if (_currentState != WandererState.Chasing)
                {
                    
                    _currentState = WandererState.Chasing;
                    
                }
                // reset chase timer
                _chaseTimeRemaining = _chaseTimer;
            }
            else if (_currentState == WandererState.Chasing && _chaseTimer < 0)
            {
                // If the player is out of range go back to wandering
                _currentState = WandererState.Wandering;

                // reset target node to pick a new one
                _targetNode = null;
                _currentNode = null;
                PathNodeSelection();

                Debug.Log("WandererEnemy lost sight of the player, returning to wandering.");
            }
        }

        /// <summary>
        /// helps with decision making
        /// </summary>
        void DecisionMaking()
        {
            switch (_currentState)
            {
                case WandererState.Wandering:
                    Wander();
                    Debug.Log("WandererEnemy is wandering.");
                    break;
                case WandererState.Chasing:
                    ChasePlayer();
                    break;
            }
        }
        #endregion

        #region Behavior Methods
        /// <summary>
        /// handles wandering behavior
        /// </summary>
        void Wander()
        {
            //gets the wander speed
            _currentSpeed = _wanderSpeed;

            // if there is no target node, pick a new one
            Debug.Log(_targetNode);
            if (_targetNode == null)
            {
                PathNodeSelection();
                return;
            }

            // Move toward the target node
            //Vector3 _directionToTarget = (_targetPosition - transform.position).normalized;
            //_rb.MovePosition(_rb.position + _directionToTarget * _currentSpeed * Time.deltaTime);

            // Set the destination to the target node's position
            Vector3 _targetPosition = _targetNode.transform.position;
            _floatingTarget = _targetPosition;

            // Check if arrived at the target node
            float _distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            if (_distanceToTarget <= _distanceToThreshold)
            {
                // Arrived at the target node, select a new one
                _currentNode = _targetNode;
                PathNodeSelection();
            }

        }

        /// <summary>
        /// handles chasing the player
        /// </summary>
        void ChasePlayer()
        {
            // chase the player
            if (_playerTransform == null) return;

            //gets the chase speed
            _currentSpeed = _chaseSpeed;

            // Move toward the player
            SetDestinationTarget(_playerTransform.position);

            // Countdown chase timer
            _chaseTimeRemaining -= Time.deltaTime;

            // If chase timer expires, return to wandering state
            if (_chaseTimeRemaining <= 0f)
            {
                // Return to wandering state
                _currentState = WandererState.Wandering;
                // reset target node to pick a new one
                _targetNode = null; 
                Debug.Log("WandererEnemy chase timer expired, returning to wandering.");
                
                // Pick a new target node to wander to
                PathNodeSelection();
            }

        }

        #endregion

        #region Path Node Selection
        /// <summary>
        /// Path Node Selection within wander radius
        /// </summary>
        void PathNodeSelection()
        {
            Debug.Log ("WandererEnemy is selecting a new target node to wander to.");
            // Select a random target node within the wander radius
            if (_currentNode == null)
            {
                _currentNode = _levelManager.GetNode(transform.position);
                if (_currentNode == null) return;
            }

            // Get all connected nodes
            var connectedNodes = new System.Collections.Generic.List<PathNode>();
            foreach (var connection in _currentNode.Connections)
            {
                // Check if the connected node is within the wander radius
                PathNode neighbor = connection.Key;
                float distanceToNeighbor = Vector3.Distance(transform.position, neighbor.transform.position);
                if (distanceToNeighbor <= _wanderRadius)
                {
                    connectedNodes.Add(neighbor);
                }
            }

            // if no connected nodes use connected nodes
            if (connectedNodes.Count == 0)
            {
                connectedNodes = new System.Collections.Generic.List<PathNode>(_currentNode.Connections.Keys);
            }

            //pick a random node from the connected nodes
            _targetNode = connectedNodes[Random.Range(0, connectedNodes.Count)];

            Debug.Log($"WandererEnemy selected new target node at {_targetNode.transform.position}");
        }
        #endregion

        #region Gizmos and Collision
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
            // Draw line to target node
            if (_targetNode != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _targetNode.transform.position);
            }
        }

        /// <summary>
        /// When the WandererEnemy collides with the player
        /// </summary>
        /// <param name="collision"></param>
        void OnCollisionEnter(Collision collision)
        {
            // Check if the collided object is the player
            if (collision.gameObject.CompareTag("Player"))
            {
                // Implement logic for when the WandererEnemy collides with the player
                Debug.Log("WandererEnemy has collided with the Player!");
            }
        }
        #endregion

    }


}
