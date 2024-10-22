using UnityEngine;

namespace SpaceGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {

        public float speed = 100f;
        public float damage = 25f;

        [Header("Explosion")]
        public GameObject explosionEffect;
        public float explosionForce = 1000f;
        public float explosionRadius = 100f;

        private Rigidbody rb;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            //transform.position += speed * Time.deltaTime * transform.forward;
        }

        void FixedUpdate()
        {
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Health health))
            {
                health.ChangeHealth(-damage);
            }

            if (collision.gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, ForceMode.Impulse);
            }

            PlayExplosionEffect();

            Destroy(gameObject);
        }

        private void PlayExplosionEffect()
        {
            var effectInstance = Instantiate(explosionEffect, transform.position, transform.rotation);

            effectInstance.TryGetComponent(out ParticleSystem effect);
            effect?.Play();
            var duration = effect?.main.duration ?? 0f;

            Destroy(effectInstance, duration);
        }
    }
}
