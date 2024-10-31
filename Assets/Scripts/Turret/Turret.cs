using UnityEngine;

namespace SpaceGame
{
    public class Turret : MonoBehaviour
    {

        protected TurretState currentState;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ChangeState(new IdleState());

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            currentState.onTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {

        }

        public void ChangeState(TurretState newState)
        {
            if (newState == null)
            {
                newState.Exit();
            }

            currentState = newState;
            newState.Enter(newState);
        }
    }
}
