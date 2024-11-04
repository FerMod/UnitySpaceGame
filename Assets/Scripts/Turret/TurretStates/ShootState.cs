using System.Linq;
using UnityEngine;

namespace SpaceGame
{
    public class ShootState : TurretState
    {

        public override void Update()
        {
            if (parent.Target != null)
            {
                parent.Rotator.LookAt(parent.Target.position + parent.AimOffset);
            }

            var canSeePlayer = parent.GunBarrels.Any((e) => parent.CanSeeTarget(e.position, e.forward, "Player"));
            Debug.Log("canSeePlayer: " + canSeePlayer);
            if (!canSeePlayer)
            {
                parent.ChangeState(new IdleState());
            }
        }
    }
}
