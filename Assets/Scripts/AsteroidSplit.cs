using System.Collections;
using UnityEngine;

namespace SpaceGame
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Health))]
    public class AsteroidSplit : MonoBehaviour
    {
        public float spawnRadius = 1f;
        public int amount = 5;
        public GameObject[] debris = { };

        [Space]
        [Tooltip("The maximum depth of splitting. When this limit is reached, the object will be destroyed instead of splitting.")]
        [Min(0)]
        public int depthLimit = 3;
        public float minDespawnTime = 5f;
        public float maxDespawnTime = 8f;

        [Header("Split Force")]
        public float radius = 100.0f;
        public float power = 5000.0f;
        public float randomTorque = 10f;

        [Header("Debug")]
        public Color color = Color.white;
        public bool showSpawnRadius = false;
        public bool showImpulseRadius = false;

        private Health health;

        void Start()
        {
            health = GetComponent<Health>();
            health.OnNoHealth += OnNoHealth;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Split();
            }
        }

        private void OnNoHealth(float oldHealth, float newHealth)
        {
            Split();
        }

        private void Split()
        {
            StartCoroutine(SplitCoroutine());
        }

        private IEnumerator SplitCoroutine()
        {
            if (debris.Length == 0) yield break;

            var reachedDepthLimit = depthLimit <= 0;

            // Iterate through the debris and spawn them.
            // If the depth limit is reached, it does not spawn more debris and instead proceeds to destroy the object
            for (var i = 0; i < amount && !reachedDepthLimit; i++)
            {
                var instance = SpawnDebris();

                var rb = instance.GetComponent<Rigidbody>();
                rb.AddExplosionForce(power, transform.position, radius, 0f, ForceMode.Impulse);
                rb.AddRelativeTorque(RandomTorque(-randomTorque, randomTorque));

                if (ShouldDestroyObject(instance, Vector3.one))
                {
                    Destroy(instance, Random.Range(minDespawnTime, maxDespawnTime));
                }
            }

            var despawnTime = 0f;
            if (reachedDepthLimit)
            {
                despawnTime = Random.Range(minDespawnTime, maxDespawnTime);
            }
            Destroy(gameObject, despawnTime);
        }

        private GameObject SpawnDebris()
        {
            var index = Random.Range(0, debris.Length);
            var instance = Instantiate(debris[index], transform.position + Random.insideUnitSphere * spawnRadius, Random.rotation);
            instance.transform.localScale = transform.localScale * Random.Range(0.2f, 0.8f);

            // Pass the depth to the debris if they also have the AsteroidSplit component
            if (instance.TryGetComponent<AsteroidSplit>(out var asteroidSplit))
            {
                asteroidSplit.depthLimit = depthLimit - 1; // Pass the decremented depth to the debris
            }

            return instance;
        }

        private bool ShouldDestroyObject(GameObject gameObject, Vector3 minScale)
        {
            if (gameObject.transform.localScale.x < minScale.x) return true;
            if (gameObject.transform.localScale.y < minScale.y) return true;
            if (gameObject.transform.localScale.z < minScale.z) return true;
            return false;
        }

        private Vector3 RandomTorque(float min, float max)
        {
            var x = Random.Range(min, max);
            var y = Random.Range(min, max);
            var z = Random.Range(min, max);
            return new Vector3(x, y, z);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;

            if (showSpawnRadius)
            {
                Gizmos.DrawWireSphere(transform.position, spawnRadius);
            }

            if (showImpulseRadius)
            {
                Gizmos.color *= 0.4f;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
}
