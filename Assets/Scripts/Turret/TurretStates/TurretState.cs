using UnityEngine;

namespace SpaceGame
{

    [System.Serializable]
    public abstract class TurretState
    {
        [SerializeField]
        public Turret parent;

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
            if (other.tag != "Player") return;
            parent.Target = null;
            parent.ChangeState(new IdleState());
        }

    }
}
