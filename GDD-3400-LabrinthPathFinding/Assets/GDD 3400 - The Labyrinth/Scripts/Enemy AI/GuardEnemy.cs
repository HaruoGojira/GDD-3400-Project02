using UnityEngine;
using UnityEngine.AI;

namespace GDD3400.Labyrinth
{
    public class GuardEnemy : EnemyAgent
    {
        //important variables
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _detectionRange = 13f;
        [SerializeField] private float _returnSpeed = 2f;
        
        //This helps to set up the guard's post
        private Vector3 _guardPost;
        private float _searchTimer;
        private Vector3 _lookDirection;
        
        // Define states for the GuardEnemy
        private enum GuardState
        {
            Return,
            Idle,
            Attacking
        }
        private GuardState _currentState;

        /// <summary>
        /// This method is called when the script instance is being loaded
        /// </summary>
        public void Awake()
        {
            //inherits the Rigidbody from EnemyAgent otherwise it wouldn't work
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }

            //this will initialize the guard's post
            _guardPost = transform.position;
            _currentState = GuardState.Idle;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            //update perception and decision making each frame
            Perception();
            DecisionMaking();
        }

        /// <summary>
        /// FixedUpdate is called at a fixed interval and is independent of frame rate
        /// Handles the movement of the guard based on its current state
        /// </summary>
        void FixedUpdate()
        {
            // Handle movement based on current state and rotation
            if (_playerTransform != null && _currentState == GuardState.Attacking)
            {
                // Move towards the player
                Vector3 _directionToPlayer = (_playerTransform.position - transform.position).normalized;
                Vector3 _movement = _directionToPlayer * _moveSpeed * Time.fixedDeltaTime;
                _rb.MovePosition(_rb.position + _movement);

                // Calculate the look direction on the horizontal plane
                _lookDirection = new Vector3(_directionToPlayer.x, 0, _directionToPlayer.z);

                // Face the guard toward the player
                if (_lookDirection != Vector3.zero)
                {
                    Quaternion _targetRotation = Quaternion.LookRotation(_lookDirection);
                    _rb.MoveRotation(Quaternion.Slerp(transform.rotation, _targetRotation, 5f * Time.fixedDeltaTime));
                }
            }
            // Walks back to guard post if in return state
            else if (_currentState == GuardState.Return)
            {
                // Move back to guard post
                Vector3 _directionToPost = (_guardPost - transform.position).normalized;
                Vector3 _movement = _directionToPost * _returnSpeed * Time.fixedDeltaTime;
                _rb.MovePosition(_rb.position + _movement);
            }
            // In Idle state, the guard does not move
            else if (_currentState == GuardState.Idle)
            {
                _rb.velocity = Vector3.zero;
            }

        }

        #region Perception and Decision Making
        /// <summary>
        /// helps with enemy perception
        /// </summary>
        void Perception()
        {
            // Check distance to player
            if (_playerTransform == null) return;

            float _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // If the player is within detection range, switch to Attacking state
            if (_distanceToPlayer <= _detectionRange)
            {
                _currentState = GuardState.Attacking;
            }
            else if (_currentState == GuardState.Attacking && _distanceToPlayer > _detectionRange)
            {
                // If the player is out of detection range, switch to Return state
                _currentState = GuardState.Return;
            }
        }

        /// <summary>
        /// helps with decision making
        /// </summary>
        void DecisionMaking()
        {
            switch (_currentState)
            {
                case GuardState.Attacking:
                    AttackPlayer();
                    break;
                case GuardState.Return:
                    ReturnToPost();
                    break;
                case GuardState.Idle:
                    Idle();
                    break;
            }
        }
        #endregion

        #region Guard States
        /// <summary>
        /// handles attacking the player and will return to idle if the player is lost
        /// </summary>
        void AttackPlayer()
        {
            // Guard attacks the player
            if (_playerTransform == null) return;

            // Move towards the player
            float _distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            //If the player is out of detection range, returns to post
            if (_distanceToPlayer > _detectionRange)
            {
                _currentState = GuardState.Return;
            }

        }

        /// <summary>
        /// handles idle behavior
        /// </summary>
        void Idle()
        {
            // guard doens't move in idle state
            _rb.velocity = Vector3.zero;
        }

        /// <summary>
        /// Guard returns to post after losing the player
        /// </summary>
        void ReturnToPost()
        {
            // Move back to guard post
            float distanceToPost = Vector3.Distance(transform.position, _guardPost);
            if (distanceToPost == 0f)
            {
                // Reached guard post, switch to Idle state
                _currentState = GuardState.Idle;
            }
        }
        #endregion

        #region Gizmos and Collision
        /// <summary>
        /// Draws gizmos to visualize detection range in the editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw detection range sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
            // Draw attack state color
            if (_currentState == GuardState.Attacking)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, _detectionRange);
            }
        }

        /// <summary>
        /// Collision detection for when the guard touches the player
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collided object is the player
            if (collision.gameObject.CompareTag("Player"))
            {
                //ends the game
                Debug.Log("Guard touched the player! Game Over.");
            }
        }

        /// <summary>
        /// if guard collides with a wall, push it slightly away
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionStay(Collision collision)
        {
            //if the guard collides with a wall, push it slightly away
            if (_currentState == GuardState.Return && collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // Calculate push direction away from the wall
                Vector3 _pushDirection = collision.contacts[0].normal;

                // Keep the push direction horizontal
                float _pushStrength = 3f;
                _rb.MovePosition(_rb.position + _pushDirection.normalized * _pushStrength * Time.fixedDeltaTime);
            }
        }
        #endregion

    }
}
