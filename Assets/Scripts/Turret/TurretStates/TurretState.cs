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

        public virtual void onTriggerEnter(Collider other)
        {

        }

    }
}
