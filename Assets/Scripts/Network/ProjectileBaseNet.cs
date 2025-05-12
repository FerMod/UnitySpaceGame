using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using System.Collections;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkRigidbody))]
    [RequireComponent(typeof(NetworkObject))]
    public abstract class ProjectileBaseNet : NetworkBehaviour
    {
        public float speed = 100f;
        public float damage = 25f;
        public float lifetime = 5f;

        public GameObject hitEffect;

        [Header("Explosion")]
        public bool isExplosive;
        public float explosionForce = 1000f;
        public float explosionRadius = 100f;
        public ForceMode forceMode = ForceMode.Impulse;

        public GameObject owner;
        public bool inheritOwnerVelocity = true;

        protected Rigidbody rb;

        public override void OnNetworkSpawn()
        {
            rb = GetComponent<Rigidbody>();
            if (!IsServer) return;
            InheritOwnerVelocity(rb);
            //Destroy(gameObject, lifetime);
            Despawn(gameObject, lifetime);
        }

        private void IgnoreColliders(GameObject owner, GameObject projectile)
        {
            if (owner == null || projectile == null) return;

            var ownerColliders = owner.GetComponentsInChildren<Collider>(includeInactive: true);
            var projectileColliders = projectile.GetComponentsInChildren<Collider>(includeInactive: true);

            if (ownerColliders.Length == 0 || projectileColliders.Length == 0) return;

            foreach (var projectileCollider in projectileColliders)
            {
                foreach (var ownerCollider in ownerColliders)
                {

                    Physics.IgnoreCollision(projectileCollider, ownerCollider);
                }
            }
        }

        private void InheritOwnerVelocity(Rigidbody rb)
        {
            if (!inheritOwnerVelocity) return;
            if (owner == null) return;
            if (!owner.TryGetComponent(out Rigidbody ownerRb)) return;
            rb.linearVelocity = ownerRb.linearVelocity;
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (!IsServer) return;
            if (collision.gameObject == owner) return;

            DamageComponent(collision.gameObject);
            PlayHitEffect();
            AddExplosionForce(collision.gameObject);

            //Destroy(gameObject);
            Despawn(gameObject);
        }

        protected void DamageComponent(GameObject gameObject)
        {
            if (gameObject == null) return;
            if (!gameObject.TryGetComponent(out HealthNet health)) return;
            health.ChangeHealth(-damage);
        }

        protected void AddExplosionForce(GameObject gameObject)
        {
            if (!isExplosive) return;
            if (gameObject == null) return;
            if (!gameObject.TryGetComponent(out Rigidbody rigidbody)) return;
            rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, forceMode);
        }

        protected void PlayHitEffect()
        {
            if (!IsServer) return;
            if (hitEffect == null) return;

            var effectInstance = Instantiate(hitEffect, transform.position, transform.rotation);
            if (effectInstance.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.Spawn();
            }

            if (!effectInstance.TryGetComponent(out ParticleSystem effect)) return;

            effect.Play();

            // Destroy(effectInstance, effect.main.duration);
            Despawn(effectInstance, effect.main.duration);
        }

        public void Despawn(GameObject gameObject, float lifetime = 0f)
        {
            if (!IsServer) return;
            if (gameObject == null) return;
            if (!gameObject.TryGetComponent(out NetworkObject networkObject))
            {
                Destroy(gameObject, lifetime);
                return;
            }

            if (lifetime > 0f)
            {
                StartCoroutine(DespawnCoroutine(networkObject, lifetime));
            }
            else
            {
                DespawnNetworkObject(networkObject);
            }
        }

        private IEnumerator DespawnCoroutine(NetworkObject networkObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            DespawnNetworkObject(networkObject);
        }

        public void DespawnNetworkObject(NetworkObject networkObject)
        {
            if (!IsServer) return;
            if (networkObject == null) return;
            if (!networkObject.IsSpawned) return;
            networkObject.Despawn();
        }
    }
}
