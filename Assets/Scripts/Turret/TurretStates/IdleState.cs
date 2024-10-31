using UnityEngine;

namespace SpaceGame
{
    public class IdleState : TurretState
    {
        public override void onTriggerEnter(Collider other)
        {
            if (other.tag != "Player") return;
            parent.ChangeState(new FindTargetState());
        }
    }
}
