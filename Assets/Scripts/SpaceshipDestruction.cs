using System.Collections;
using UnityEngine;

namespace SpaceGame
{
    public class SpaceshipDestruction : MonoBehaviour
    {
        public Plane plane;

        /// <summary>
        /// The parent of all the spaceship parts.
        /// </summary>
        public Transform spaceship;

        public float explosionForce = 500f;
        public float explosionRadius = 5f;
        public Vector3 explosionCenterOffset = Vector3.zero;

        [Header("Effects")]
        public GameObject bigExplosion;
        public GameObject mediumExplosion;
        public GameObject smallExplosion;

        void Start()
        {
            plane.GetComponent<Health>().OnNoHealth += OnNoHealth;
        }

        private void OnNoHealth(object source, float oldHealth, float newHealth)
        {
            Die();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                plane.GetComponent<Health>().ChangeHealth(-9999);

            }
        }

        public void Die()
        {

            PlayEffect(bigExplosion, transform.position, transform.rotation);

            //PlayEffectsRandom(1, 4, 0.1f, 1f);

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
        IEnumerator DelayAction(Transform transform, float delayTime)
        {
            //Wait for the specified delay time before continuing.
            yield return new WaitForSeconds(delayTime);

            PlayEffect(smallExplosion, transform.position, transform.rotation);
        }


        private void PlayEffectsRandom(int min, int max, float minDelay, float maxDelay)
        {
            StartCoroutine(PlayEffectsRandomCouroutine(min, max, minDelay, maxDelay));
        }

        private IEnumerator PlayEffectsRandomCouroutine(int min, int max, float minDelay, float maxDelay)
        {
            // Generate a random number of times to play the effect.
            var randomTimes = Random.Range(min, max + 1);
            for (var i = 0; i < randomTimes; i++)
            {
                // Wait for a random delay before playing the effect.
                var randomDelay = Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(randomDelay);

                // Call the PlayEffect method.
                PlayEffect(mediumExplosion, transform.position, transform.rotation);
            }
        }

    }
}
