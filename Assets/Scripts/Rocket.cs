using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace SpaceGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class RocketProjectile : Projectile
    {

        public bool isGuided = true;
        public Transform target;

        public bool HasTarget => target != null;

        //void Update()
        //{
        //    if (isGuided)
        //    {

        //        // Rotate the rocket to face the target
        //        LookTowards(target);

        //        // Move the rocket forward
        //        rb.linearVelocity = transform.forward * speed;
        //    }
        //}

        void FixedUpdate()
        {

            if (isGuided)
            {
                LookTowards(target);
            }
        }

        void LookTowards(Transform target)
        {
            if (!HasTarget) return;
            Assert.IsNotNull(target, "Target is null");
            transform.LookAt(target);
        }

    }
}
