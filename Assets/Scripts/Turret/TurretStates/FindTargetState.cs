using UnityEngine;

namespace SpaceGame
{
    public class FindTargetState : TurretState
    {
        public override void onTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {

            }
        }
    }
}
