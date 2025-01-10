using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.UI.GridLayoutGroup;

namespace SpaceGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class RocketProjectile : ProjectileBase
    {

        public GameObject smokeTrail;
        private ParticleSystem smokeTrailParticles;

        public bool isGuided = true;
        public float trackingAngle = 80f;
        public float turningGForce = 2f;
        public Transform target;


        public bool HasTarget => target != null;

        //void Update()
        //{
        //    if (isGuided)
        //    {

        //        // Rotate the rocket to face the target
        //        LookTowards(target);

        //        // Move the rocket forward
        //        rb.linearVelocity += speed * Time.deltaTime * transform.forward;
        //    }
        //}

        protected new void Start()
        {
            base.Start();
            smokeTrailParticles = smokeTrail.GetComponent<ParticleSystem>();

        }

        void FixedUpdate()
        {
            //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

            if (isGuided)
            {
                LookTowards(Time.fixedDeltaTime, target, turningGForce);
            }

            rb.linearVelocity = rb.rotation * new Vector3(0, 0, speed);
        }

        protected new void OnCollisionEnter(Collision collision)
        {
            if (smokeTrailParticles != null)
            {
                // Detach the smoke trail and stop emitting smoke
                smokeTrailParticles.transform.parent = null;
                smokeTrailParticles.Stop();
            }

            base.OnCollisionEnter(collision);
        }


        void LookTowards(float dt, Transform target, float turningGForce)
        {
            if (!HasTarget) return;
            Assert.IsNotNull(target, "Target is null");

            var targetPosition = Utils.FirstOrderIntercept(rb.position, Vector3.zero, speed, target.position, Vector3.zero);

            var error = targetPosition - rb.position;
            var targetDir = error.normalized;
            var currentDir = rb.rotation * Vector3.forward;

            // if angle to target is too large, explode
            if (Vector3.Angle(currentDir, targetDir) > trackingAngle)
            {
                PlayHitEffect();
                Destroy(gameObject);
                //Explode();
                return;
            }

            var maxTurnRate = (turningGForce * 9.81f) / speed;  // radians / s
            var dir = Vector3.RotateTowards(currentDir, targetDir, maxTurnRate * dt, 0);

            rb.rotation = Quaternion.LookRotation(dir);
        }

    }
}
