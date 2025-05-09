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

        [SerializeField]
        private bool isGuided = true;

        [SerializeField]
        private float trackingAngle = 80f;

        [SerializeField]
        private float turningGForce = 2f;

        [SerializeField]
        private Transform target;

        public bool HasTarget => target != null;

        // Network variables
        private NetworkVariable<bool> IsGuidedNet = new(true);
        private NetworkVariable<Vector3> TargetPositionNet = new(Vector3.zero);

        protected new void Start()
        {
            smokeTrailParticles = smokeTrail.GetComponent<ParticleSystem>();

            if (IsServer)
            {
                IsGuidedNet.Value = isGuided;

                if (HasTarget)
                {
                    TargetPositionNet.Value = target.position;
                }
            }
        }

        void FixedUpdate()
        {
            if (!IsServer) return;

            //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

            if (IsGuidedNet.Value && HasTarget)
            {
                TargetPositionNet.Value = target.position; // Live tracking
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

            HandleCollisionClientRpc();

            base.OnCollisionEnter(collision);
        }

        [ClientRpc]
        private void HandleCollisionClientRpc()
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
                HandleCollisionClientRpc();
                Despawn(gameObject);
                //Destroy(gameObject);
                //Explode();
                return;
            }

            var maxTurnRate = (turningGForce * 9.81f) / speed;  // radians / s
            var dir = Vector3.RotateTowards(currentDir, targetDir, maxTurnRate * dt, 0);

            rb.rotation = Quaternion.LookRotation(dir);
        }

        // Network setters
        [ServerRpc]
        public void SetTargetServerRpc(Vector3 targetPosition)
        {
            TargetPositionNet.Value = targetPosition;
        }

        [ServerRpc]
        public void SetIsGuidedServerRpc(bool isGuided)
        {
            IsGuidedNet.Value = isGuided;
        }
    }
}
