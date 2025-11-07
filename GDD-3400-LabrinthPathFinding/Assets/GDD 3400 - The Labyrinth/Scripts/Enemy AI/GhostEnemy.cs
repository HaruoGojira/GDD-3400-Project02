using UnityEngine;
using UnityEngine.AI;

namespace GDD3400.Labyrinth
{
    public class GhostEnemy : EnemyAgent
    {
        //important variables
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _moveSpeed = 6f;
        private Vector3 _lookDirection;

        /// <summary>
        /// This method is called when the script instance is being loaded
        /// </summary>
        public void Awake()
        {
            //inherits the Rigidbody from EnemyAgent otherwise it would override it
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }

            //use a ghost layer to ignore wall layer collisions
            int ghostLayer = LayerMask.NameToLayer("Ghost");
            int wallLayer = LayerMask.NameToLayer("Wall");
            Physics.IgnoreLayerCollision(ghostLayer, wallLayer, true);
        }

        // Update is called once per frame
        void Update()
        {
            // the ghosts will always float toward the player
            if (_playerTransform == null) return;

            //the ghost will move toward the player
            Vector3 _directionToPlayer = (_playerTransform.position - transform.position).normalized;
            Vector3 _movement = _directionToPlayer * _moveSpeed * Time.deltaTime;
            _rb.MovePosition(_rb.position + _movement);

            // Calculate the look direction on the horizontal plane
            _lookDirection = new Vector3(_directionToPlayer.x, 0, _directionToPlayer.z);
            
            // Face the ghost toward the player
            if (_lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
                _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 5f));
            }

        }

        /// <summary>
        /// collision if the ghost touches the player
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collided object is the player
            if (collision.gameObject.CompareTag("Player"))
            {
                //ends the game
                Debug.Log("Ghost touched the player! Game Over.");

            }
        }

    }
}
