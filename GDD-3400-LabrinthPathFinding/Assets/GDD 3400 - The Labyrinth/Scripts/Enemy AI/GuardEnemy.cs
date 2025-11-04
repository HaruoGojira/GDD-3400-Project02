using UnityEngine;

namespace GDD3400.Labyrinth
{
    public class GuardEnemy : EnemyAgent
    {
        //important variables
        private Vector3 _searchPosition;
        private float _searchTimer;

        // Define states for the StalkerEnemy
        private enum GuardState
        {
            Wander,
            Idle,
            Attacking
        }
        private GuardState _currentState;

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
            if (_currentState == GuardState.Idle)
            {
                // Logic for detecting the player and transitioning to Stalking state

            }
            
            else if (_currentState == GuardState.Attacking)
            {
                // Logic for attacking behavior
            }
        }

        //helps with decision making
        void DecisionMaking()
        {
            switch (_currentState)
            {
                case GuardState.Idle:
                    Idle();
                    break;
                case GuardState.Attacking:
                    AttackPlayer();
                    break;
                case GuardState.Wander:
                    Wander();
                    break;
            }
        }

        //handles the wandering until it sees the player
        void Wander()
        {
            if (_currentState == GuardState.Idle)
            {
                //wanders until it sees the player
                
            }
        }

       

        //handles attacking the player
        void AttackPlayer()
        {
            
        }

        //handles idle behavior
        void Idle()
        {
            if (_currentState == GuardState.Attacking)
            {
                // Logic for transitioning to Idle state

            }
            else if (_currentState == GuardState.Idle)
            {
                // Logic for idle behavior
            }
        }
    }
}
