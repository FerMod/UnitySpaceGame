using SpaceGame.Network;
using System.Collections;
using UnityEngine;

namespace SpaceGame
{
    public class SpaceshipDestruction : MonoBehaviour
    {
        public PlaneNet plane;

        /// <summary>
        /// The parent of all the spaceship parts.
        /// </summary>
        public Transform spaceship;

        [Header("Explosion")]
        public GameObject explosionEffect;
        public float explosionForce = 100f;
        public float explosionRadius = 5f;
        public Vector3 explosionCenterOffset = Vector3.zero;

        void Start()
        {
            plane.GetComponent<HealthNet>().OnNoHealth += OnNoHealth;
        }

        private void OnNoHealth(float oldHealth, float newHealth)
        {
            Die();
        }

        public void Die()
        {

            PlayEffect(explosionEffect, transform.position, transform.rotation);

            // Unparent all children
            var childCount = spaceship.transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = spaceship.transform.GetChild(i);
                // Detach the child from the parent
                child.SetParent(null);

                // Add Rigidbody if it doesn't already have one
                var rb = child.gameObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = child.gameObject.AddComponent<Rigidbody>();
                }
                rb.useGravity = false;

                rb.AddExplosionForce(explosionForce, explosionCenterOffset, explosionRadius);
                //rb.AddForce(Random.insideUnitSphere * explosionForce);
                rb.AddTorque(Random.insideUnitSphere * explosionForce);
            }

            // Destroy the spaceship itself
            Destroy(plane);
        }

        private void PlayEffect(GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            if (gameObject == null) return;

            var effectInstance = Instantiate(gameObject, position, rotation);
            effectInstance.TryGetComponent(out ParticleSystem effect);
            if (effect == null) return;

            effect.Play();

            Destroy(effectInstance, effect.main.duration);
        }
    }
}
