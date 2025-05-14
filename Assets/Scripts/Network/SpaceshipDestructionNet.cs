using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(HealthNet))]
    public class SpaceshipDestructionNet : NetworkBehaviour
    {
        /// <summary>
        /// The parent of all the spaceship parts.
        /// </summary>
        public Transform spaceship;

        public float debriDespawnTime = 10f;

        [Header("Explosion")]
        public GameObject explosionEffect;
        public float explosionForce = 100f;
        public float explosionRadius = 5f;
        public Vector3 explosionCenterOffset = Vector3.zero;

        [Header("Respawn")]
        public InputActionAsset inputActionAsset;
        public float respawnDelay = 3f;
        public Vector3 respawnPosition = Vector3.zero;

        private HealthNet health;

        public override void OnNetworkSpawn()
        {
            health = GetComponent<HealthNet>();
            health.OnNoHealth += HandleNoHealth;
        }

        private void HandleNoHealth(float oldHealth, float newHealth)
        {
            Die();
        }

        public void Die()
        {
            spaceship.gameObject.SetActive(false);

            var obj = Instantiate(spaceship.gameObject, transform.position, transform.rotation);
            if (obj.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.Spawn();
            }

            PlayEffect(explosionEffect, transform.position, transform.rotation);

            // Unparent all children
            var childCount = obj.transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = obj.transform.GetChild(i);
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

                // Destroy the child after a delay
                Destroy(child.gameObject, debriDespawnTime);
            }

            Respawn(respawnDelay);
        }

        private void PlayEffect(GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            if (gameObject == null) return;

            var effectInstance = Instantiate(gameObject, position, rotation);

            if (!effectInstance.TryGetComponent(out ParticleSystem effect)) return;

            effect.Play();

            Destroy(effectInstance, effect.main.duration);
        }

        private void Respawn(float delay = 3f)
        {
            inputActionAsset.Disable();

            var planeNet = GetComponent<PlaneNet>();
            planeNet.Throttle = 0f;
            planeNet.Rigidbody.linearVelocity = Vector3.zero;
            planeNet.Rigidbody.angularVelocity = Vector3.zero;

            StartCoroutine(RespawnAfterDelay(delay));
        }

        private IEnumerator RespawnAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            health.ChangeHealth(health.MaxHealth);

            gameObject.transform.SetPositionAndRotation(respawnPosition, Quaternion.Euler(Vector3.forward));
            spaceship.gameObject.SetActive(true);

            inputActionAsset.Enable();
        }

        /*private void Update()
        {
            if (IsServer && Input.GetKeyDown(KeyCode.K))
            {
                health.ChangeHealth(-health.MaxHealth);
            }
        }*/
    }
}
