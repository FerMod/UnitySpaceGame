using UnityEngine;

namespace SpaceGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
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

        protected Rigidbody rb;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected void Start()
        {
            rb = GetComponent<Rigidbody>();

            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            //transform.position += speed * Time.deltaTime * transform.forward;
            //rb.linearVelocity += speed * Time.deltaTime * transform.forward;
        }

        void FixedUpdate()
        {
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }

        void OnCollisionEnter(Collision collision)
        {
            DamageComponent(collision);

            PlayHitEffect();
            AddExplosionForce(collision);

            Destroy(gameObject);
        }

        private void DamageComponent(Collision collision)
        {
            collision.gameObject.TryGetComponent(out Health health);
            if (health == null) return;

            health.ChangeHealth(-damage);
        }

        private void AddExplosionForce(Collision collision)
        {
            if (!isExplosive) return;

            collision.gameObject.TryGetComponent(out Rigidbody rigidbody);
            if (rigidbody == null) return;

            rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, forceMode);
        }

        private void PlayHitEffect()
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
