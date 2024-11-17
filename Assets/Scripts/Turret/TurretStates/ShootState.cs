using System.Linq;
using UnityEngine;

namespace SpaceGame
{

    [System.Serializable]
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
            if (parent.Target == null)
            {
                parent.ChangeState(new IdleState());
                return;
            }


            //parent.Rotator.LookAt(parent.TargetTransform.position + parent.AimOffset);


            // === derived variables ===
            // Positions
            Vector3 shooterPosition = parent.Gun.projectileSpawnPoints[0].position;
            Vector3 targetPosition = parent.Target.transform.position + parent.AimOffset;

            // Velocities
            Vector3 shooterVelocity = parent.GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
            var projectileSpeed = parent.Gun.projectile.GetComponent<Projectile>().speed;
            //Vector3 targetVelocity = parent.Target.GetComponent<Plane>().Velocity;
            Vector3 targetVelocity = parent.Target.GetComponent<DebugScript>().Velocity;

            //calculate intercept
            Vector3 interceptPoint = Utils.FirstOrderIntercept(shooterPosition, shooterVelocity, projectileSpeed, targetPosition, targetVelocity);

            parent.LookAt(interceptPoint);

            Debug.DrawLine(shooterPosition, interceptPoint, Color.magenta, 0.05f);
            parent.Gun.Fire();

            if (!HasDirectSight() && !HasUnobstructedPath(interceptPoint, Color.green))
            {
                Debug.Log("No direct line of sight");
                // parent.ChangeState(new IdleState());
            }
            else
            {
            }
        }

        private bool HasUnobstructedPath(Vector3 toPosition, Color? color = null)
        {
            return parent.HasUnobstructedSight(toPosition, color);
        }

        private bool HasDirectSight(Color? color = null)
        {
            return parent.GunBarrels.Any((e) => parent.RaycastTarget(e.position, e.forward, "Player", color));
            //return parent.RaycastTarget(parent.Rotator.position, parent.GunBarrels[0].forward, "Player", color);
        }
    }
}
