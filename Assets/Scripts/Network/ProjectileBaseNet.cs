using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (!IsServer) return;
            InheritOwnerVelocity(rb);

            //Destroy(gameObject, lifetime);
            Despawn(lifetime);
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

            DamageComponent(collision.gameObject);

            PlayHitEffect();
            AddExplosionForce(collision.gameObject);

            //Destroy(gameObject);
            Despawn();
        }

        public void Despawn(float lifetime = 0f)
        {
            if (lifetime >= 0)
            {
                Invoke(nameof(DespawnNetworkObject), lifetime);
            }
            else
            {
                DespawnNetworkObject();
            }

        }

        public void DespawnNetworkObject()
        {
            if (!IsServer) return;
            if (!TryGetComponent(out NetworkObject networkObject)) return;
            networkObject.Despawn();
        }

        protected void DamageComponent(GameObject gameObject)
        {
            if (gameObject == null) return;
            gameObject.TryGetComponent(out HealthNet health);

            if (health == null) return;
            health.ChangeHealth(-damage);
        }

        protected void AddExplosionForce(GameObject gameObject)
        {
            if (!isExplosive) return;

            if (gameObject == null) return;
            gameObject.TryGetComponent(out Rigidbody rigidbody);

            if (rigidbody == null) return;
            rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, forceMode);
        }

        protected void PlayHitEffect()
        {
            if (hitEffect == null) return;

            var effectInstance = Instantiate(hitEffect, transform.position, transform.rotation);
            effectInstance.TryGetComponent(out ParticleSystem effect);
            if (effect == null) return;

            effect.Play();

            Destroy(effectInstance, effect.main.duration);
        }
    }
}
