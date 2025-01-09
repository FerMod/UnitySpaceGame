using UnityEngine;

namespace SpaceGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : ProjectileBase
    {
        void Update()
        {
            //transform.position += speed * Time.deltaTime * transform.forward;
            //rb.linearVelocity += speed * Time.deltaTime * transform.forward;
        }

        void FixedUpdate()
        {
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }
    }
}
