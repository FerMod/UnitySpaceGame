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
                parent.Rotator.LookAt(parent.Target.position + parent.AimOffset);
            }

            if (CanShootPlayer())
            {
                parent.Gun.Fire();
            }
            else
            {
                parent.ChangeState(new IdleState());
            }
        }

        private bool CanShootPlayer(Color? color = null)
        {
            return parent.GunBarrels.Any((e) => parent.RaycastTarget(e.position, e.forward, "Player", color));
            // return parent.RaycastTarget(parent.Rotator.position, parent.GunBarrels[0].forward, "Player", color);
        }
    }
}
