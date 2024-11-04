using UnityEngine;

namespace SpaceGame
{
    public abstract class TurretState
    {

        protected Turret parent;

        public virtual void Enter(Turret parent)
        {
            this.parent = parent;
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void OnTriggerEnter(Collider other)
        {
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                parent.Target = null;
                parent.ChangeState(new IdleState());
            }
        }

    }
}
