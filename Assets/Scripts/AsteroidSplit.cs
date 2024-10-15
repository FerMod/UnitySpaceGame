using UnityEngine;

namespace SpaceGame
{

    public class AsteroidSplit : MonoBehaviour
    {

        public float radius = 100.0f;
        public float power = 5000.0f;

        public float spawnRadius = 1f;
        public int amount = 5;
        public GameObject[] debris = { };

        public bool showSpawnRadius = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Split();
            }

        }

        void Split()
        {
            if (debris.Length == 0) return;

            for (int i = 0; i < amount; i++)
            {

                var instance = SpawnDebris();
                instance.transform.localScale *= Random.Range(0.2f, 0.8f);

                // TODO: This logic should be handled on the proyectile logic
                var rb = instance.GetComponent<Rigidbody>();
                rb.AddExplosionForce(power, transform.position, spawnRadius * 100f, 0f, ForceMode.Impulse);
                rb.AddRelativeTorque(new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f)));
                // rb.angularVelocity = Random.insideUnitSphere * 0.9f;

                Destroy(instance, Random.Range(5f, 8f));
            }

            Destroy(gameObject);
        }
        private GameObject SpawnDebris()
        {
            var index = Random.Range(0, debris.Length);
            return Instantiate(debris[index], transform.position + Random.insideUnitSphere * spawnRadius, Random.rotation);
        }


        private void OnDrawGizmos()
        {
            if (!showSpawnRadius) return;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
