using System.Linq;
using UnityEngine;

namespace SpaceGame
{
    public class ShootState : TurretState
    {
        public override void Enter(Turret parent)
        {
            base.Enter(parent);

            parent.Animator.SetBool("Shoot", true);
        }

        public override void Exit()
        {
            parent.Animator.SetBool("Shoot", false);
        }

        public override void Update()
        {
            if (parent.Target != null)
            {
                //parent.Rotator.LookAt(parent.TargetTransform.position + parent.AimOffset);

                var targetSpeed = parent.Target.GetComponent<Rigidbody>().linearVelocity;
                var projectileSpeed = parent.Gun.projectile.GetComponent<Projectile>().speed;
                /*
                 var leadingVector = Utils.PredictV3Pos(parent.GunBarrels[0].position, projectileSpeed, parent.TargetTransform.position, targetSpeed * 0.9f);
                 Debug.DrawLine(parent.GunBarrels[0].position, leadingVector, Color.magenta, 0.1f);
                 parent.Rotator.LookAt(leadingVector);
                */

                // === derived variables ===
                //positions
                Vector3 shooterPosition = parent.GunBarrels[0].position;
                Vector3 targetPosition = parent.Target.transform.position;
                //velocities
                Vector3 shooterVelocity = Vector3.zero;
                Vector3 targetVelocity = targetSpeed;

                //calculate intercept
                Vector3 interceptPoint = Utils.FirstOrderIntercept(shooterPosition, shooterVelocity, projectileSpeed, targetPosition, targetVelocity);
                parent.Rotator.LookAt(interceptPoint);
                Debug.DrawLine(shooterPosition, interceptPoint, Color.magenta, 0.1f);

                parent.Gun.Fire();
            }

            if (!HasDirectSight())
            {
                parent.ChangeState(new IdleState());
            }
        }

        private bool HasDirectSight(Color? color = null)
        {
            return parent.GunBarrels.Any((e) => parent.RaycastTarget(e.position, e.forward, "Player", color));
            // return parent.RaycastTarget(parent.Rotator.position, parent.GunBarrels[0].forward, "Player", color);
        }
    }
}
