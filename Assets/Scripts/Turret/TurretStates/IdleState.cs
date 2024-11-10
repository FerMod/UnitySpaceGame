using System.Linq;
using UnityEngine;

namespace SpaceGame
{
    [System.Serializable]
    public class IdleState : TurretState
    {
        public override void Update()
        {
            if (parent.DefaultRotation != parent.Rotator.rotation)
            {
                parent.Rotator.rotation = Quaternion.RotateTowards(parent.Rotator.rotation, parent.DefaultRotation, Time.deltaTime * parent.RotationSpeed);
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
