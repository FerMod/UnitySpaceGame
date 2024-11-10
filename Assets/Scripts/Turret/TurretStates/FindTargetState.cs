using System;
using UnityEngine;

namespace SpaceGame
{
    public class FindTargetState : TurretState
    {
        public override void Update()
        {
            RotateTowardsTarget();

            if (parent.GhostRotator.rotation.y == parent.Rotator.rotation.y)
            {
                parent.ChangeState(new ShootState());
            }
        }

        private void RotateTowardsTarget()
        {
            parent.GhostRotator.LookAt(parent.Target.transform.position + parent.AimOffset);
            parent.Rotator.rotation = Quaternion.RotateTowards(parent.Rotator.rotation, parent.GhostRotator.rotation, Time.deltaTime * parent.RotationSpeed);
        }

    }
}
