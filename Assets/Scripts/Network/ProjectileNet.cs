using UnityEngine;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileNet : ProjectileBaseNet
    {
        void FixedUpdate()
        {
            if (!IsServer) return;
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }
    }
}
