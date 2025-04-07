using System.Linq;
using UnityEngine;

namespace SpaceGame
{

    [System.Serializable]
    public class IdleState : TurretState
    {
        public override void Update()
        {
            if (parent.DefaultRotation != parent.HorizontalRotator.rotation)
            {
                //parent.HorizontalRotator.rotation = Quaternion.RotateTowards(parent.HorizontalRotator.rotation, parent.DefaultRotation, Time.deltaTime * parent.RotationSpeed);
                //parent.HorizontalRotator.rotation = parent.HorizontalRotator.rotation.RotateAxisTowards(parent.DefaultRotation, Time.deltaTime * parent.RotationSpeed, Axis.Y);
                parent.Rotate(parent.DefaultRotation);
            }

            if (parent.Target != null && CanSeePlayer())
            {
                parent.ChangeState(new FindTargetState());
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player") return;
            parent.Target = other.gameObject;
            parent.ChangeState(new FindTargetState());
        }

        private bool CanSeePlayer(Color? color = null)
        {
            return parent.GunBarrels.Any((e) => parent.RaycastTarget(e.position, parent.Target.transform.position + parent.AimOffset - e.position, "Player", color));
        }
    }
}
