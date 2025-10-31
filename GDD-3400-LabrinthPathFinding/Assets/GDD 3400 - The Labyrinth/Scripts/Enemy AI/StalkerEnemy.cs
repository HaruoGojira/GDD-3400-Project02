using UnityEngine;

namespace GDD3400.Labyrinth
{
    public class StalkerEnemy : EnemyAgent
    {
        //important variables
        private Vector3 _searchPosition;
        private float _searchTimer;

        // Define states for the StalkerEnemy
        private enum StalkerState
        {
            Wander,
            Idle,
            Stalking,
            Attacking
        }
        private StalkerState _currentState;

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
            //update perception and decision making each frame
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
            // Logic for perceiving the player and environment
            if (_currentState == StalkerState.Idle)
            {
                // Logic for detecting the player and transitioning to Stalking state

            }
            else if (_currentState == StalkerState.Stalking)
            {
                // Logic for maintaining stalking behavior
            }
            else if (_currentState == StalkerState.Attacking)
            {
                // Logic for attacking behavior
            }
        }

        //helps with decision making
        void DecisionMaking()
        {
            switch (_currentState)
            {
                case StalkerState.Idle:
                    Idle();
                    break;
                case StalkerState.Stalking:
                    StalkPlayer();
                    break;
                case StalkerState.Attacking:
                    AttackPlayer();
                    break;
                case StalkerState.Wander:
                    Wander();
                    break;
            }
        }

        //handles the wandering until it sees the player
        void Wander()
        {
            if (_currentState == StalkerState.Idle)
            {
                //wanders until it sees the player
                
            }
        }

        //handles stalking the player
        void StalkPlayer()
        {
            if (_currentState == StalkerState.Idle)
            {
                // Logic for transitioning to Stalking state
            }
            else if (_currentState == StalkerState.Stalking)
            {
                // Logic for stalking behavior
            }
        }

        //handles attacking the player
        void AttackPlayer()
        {
            if (_currentState == StalkerState.Stalking)
            {
                // Logic for transitioning to Attacking state
            }
            else if (_currentState == StalkerState.Attacking)
            {
                // Logic for attacking behavior
            }
        }

        //handles idle behavior
        void Idle()
        {
            if (_currentState == StalkerState.Attacking)
            {
                // Logic for transitioning to Idle state

            }
            else if (_currentState == StalkerState.Idle)
            {
                // Logic for idle behavior
            }
        }
    }
}
