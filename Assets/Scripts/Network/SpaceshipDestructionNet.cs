using Unity.Netcode;
using UnityEngine;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(HealthNet))]
    public class SpaceshipDestructionNet : NetworkBehaviour
    {
        /// <summary>
        /// The parent of all the spaceship parts.
        /// </summary>
        public Transform spaceship;

        [Header("Explosion")]
        public GameObject explosionEffect;
        public float explosionForce = 100f;
        public float explosionRadius = 5f;
        public Vector3 explosionCenterOffset = Vector3.zero;

        public override void OnNetworkSpawn()
        {
            GetComponent<HealthNet>().OnNoHealth += OnNoHealth;
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
                if (!child.gameObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb = child.gameObject.AddComponent<Rigidbody>();
                }
                rb.useGravity = false;

                rb.AddExplosionForce(explosionForce, explosionCenterOffset, explosionRadius);
                //rb.AddForce(Random.insideUnitSphere * explosionForce);
                rb.AddTorque(Random.insideUnitSphere * explosionForce);
            }

            // Destroy the spaceship itself
            Debug.Log("Spaceship destroyed");
            //Destroy(spaceship.gameObject);
        }

        private void PlayEffect(GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            if (gameObject == null) return;

            var effectInstance = Instantiate(gameObject, position, rotation);

            if (!effectInstance.TryGetComponent(out ParticleSystem effect)) return;

            effect.Play();

            Destroy(effectInstance, effect.main.duration);
        }
    }
}
