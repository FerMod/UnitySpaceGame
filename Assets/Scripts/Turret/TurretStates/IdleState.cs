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

            if (parent.Target != null)
            {
                var canSeeTarget = parent.GunBarrels.Any((e) => parent.CanSeeTarget(e.position, parent.Target.position + parent.AimOffset - e.position, "Player"));
                if (canSeeTarget)
                {
                    parent.ChangeState(new FindTargetState());
                }
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
