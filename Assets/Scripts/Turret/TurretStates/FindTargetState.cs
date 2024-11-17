using UnityEngine;

namespace SpaceGame
{

    [System.Serializable]
    public class FindTargetState : TurretState
    {


        public override void Update()
        {
            RotateTowardsTarget();

            if (CanShootTarget(parent.GhostRotator.rotation, parent.HorizontalRotator.rotation))
            {
                parent.ChangeState(new ShootState());
            }
        }

        private bool CanShootTarget(Quaternion fromRotation, Quaternion toRotation)
        {
            if (!IsWithinTolerance(fromRotation.x, toRotation.x, parent.ShootTolerance))
            {
                return false;
            }
            if (!IsWithinTolerance(fromRotation.y, fromRotation.y, parent.ShootTolerance))
            {
                return false;
            }
            if (!IsWithinTolerance(fromRotation.z, fromRotation.z, parent.ShootTolerance))
            {
                return false;
            }
            return true;
        }

        private bool IsWithinTolerance(float fromAngle, float toAngle, float tolerance)
        {
            return Mathf.Abs(Mathf.DeltaAngle(fromAngle, toAngle)) <= tolerance;
        }

        private void RotateTowardsTarget()
        {
            parent.GhostRotator.LookAt(parent.Target.transform.position + parent.AimOffset);

            //parent.HorizontalRotator.rotation = Quaternion.RotateTowards(parent.HorizontalRotator.rotation, parent.GhostRotator.rotation, Time.deltaTime * parent.RotationSpeed);
            parent.Rotate(parent.GhostRotator.rotation);
        }

    }
}
