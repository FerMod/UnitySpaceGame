using System.Linq;
using UnityEngine;

namespace SpaceGame
{
    public class IdleState : TurretState
    {
        public override void Update()
        {
            if (parent.DefaultRotation != parent.Rotator.rotation)
            {
                parent.Rotator.rotation = Quaternion.RotateTowards(parent.Rotator.rotation, parent.DefaultRotation, Time.deltaTime * parent.RotationSpeed);
            }

            if (parent.Target != null && parent.CanSeeTarget())
            {
                parent.ChangeState(new FindTargetState());
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player") return;
            parent.Target = other.transform;
            parent.ChangeState(new FindTargetState());
        }
    }
}
