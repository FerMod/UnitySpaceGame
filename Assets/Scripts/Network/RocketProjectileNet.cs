using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(Rigidbody))]
    public class RocketProjectileNet : ProjectileBaseNet
    {
        public GameObject smokeTrail;
        private ParticleSystem smokeTrailParticles;

        public bool isGuided = true;
        public float trackingAngle = 80f;
        public float turningGForce = 2f;
        public Transform target;

        public bool HasTarget => target != null;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            smokeTrailParticles = smokeTrail.GetComponent<ParticleSystem>();

            if (IsServer)
            {
                //var testTarget = GameObject.Find("MercuryMoon");
                //if (testTarget != null)
                //{
                //    target = testTarget.transform;
                //}
                SearchTarget();
            }
        }

        private void SearchTarget(float maxDistance = Mathf.Infinity)
        {
            if (target != null) return;
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, maxDistance)) return;
            if (!hit.collider.CompareTag("Player")) return;
            target = hit.collider.transform;
        }

        void FixedUpdate()
        {
            if (!IsServer) return;

            //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

            if (isGuided && HasTarget)
            {
                target = target.transform; // Live tracking
                LookTowards(Time.fixedDeltaTime, target, turningGForce);
            }

            //rb.AddRelativeForce(transform.forward * speed, ForceMode.Impulse);
            rb.linearVelocity += rb.rotation * new Vector3(0, 0, speed);
        }

        //void Update()
        //{
        //    if (!IsServer) return;
        //    if (isGuided)
        //    {
        //        // Rotate the rocket to face the target
        //        LookTowards(target);
        //
        //        // Move the rocket forward
        //        rb.linearVelocity += speed * Time.deltaTime * transform.forward;
        //    }
        //}

        protected new void OnCollisionEnter(Collision collision)
        {
            if (!IsServer) return;
            if (collision.gameObject == owner) return;

            if (smokeTrailParticles != null)
            {
                // Detach the smoke trail and stop emitting smoke
                smokeTrailParticles.transform.parent = null;
                smokeTrailParticles.Stop();
            }

            HandleCollisionRpc();

            base.OnCollisionEnter(collision);
        }

        [Rpc(SendTo.NotServer)]
        private void HandleCollisionRpc()
        {
            if (smokeTrailParticles != null)
            {
                smokeTrailParticles.transform.parent = null;
                smokeTrailParticles.Stop();
            }
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
                HandleCollisionRpc();
                Despawn(gameObject);
                //Destroy(gameObject);
                //Explode();
                return;
            }

            var maxTurnRate = (turningGForce * 9.81f) / speed;  // radians / s
            var dir = Vector3.RotateTowards(currentDir, targetDir, maxTurnRate * dt, 0);

            rb.rotation = Quaternion.LookRotation(dir);
        }
    }
}
