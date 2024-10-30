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

        [Header("Debug")]
        public bool showSpawnRadius = false;

        private Health health;

        [Header("TEST")]
        public float radius = 100.0f;
        public float power = 5000.0f;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            health = GetComponent<Health>();
            health.OnNoHealth += OnNoHealth;
        }

        private void OnNoHealth(object source, float oldHealth, float newHealth)
        {
            Split();
        }

        void Split()
        {
            if (debris.Length == 0) return;

            for (int i = 0; i < amount; i++)
            {

                var instance = SpawnDebris();

                // TODO: This logic should be handled on the proyectile logic
                var rb = instance.GetComponent<Rigidbody>();

                rb.AddExplosionForce(power, transform.position, spawnRadius * 100f, 0f, ForceMode.Impulse);
                rb.AddRelativeTorque(RandomTorque(-10f, 10f));
                // rb.angularVelocity = Random.insideUnitSphere * 0.9f;

                if (ShouldDestroyObject(instance, Vector3.one))
                {
                    Destroy(instance, Random.Range(5f, 8f));
                }
            }

            Destroy(gameObject);
        }
        private GameObject SpawnDebris()
        {
            var index = Random.Range(0, debris.Length);
            var instance = Instantiate(debris[index], transform.position + Random.insideUnitSphere * spawnRadius, Random.rotation);
            instance.transform.localScale = transform.localScale * Random.Range(0.2f, 0.8f);
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
            if (!showSpawnRadius) return;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
