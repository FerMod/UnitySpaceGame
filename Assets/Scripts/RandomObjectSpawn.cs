using UnityEngine;

namespace SpaceGame
{

    public class RandomObjectSpawn : MonoBehaviour
    {
        public int seed = 0;

        [Space(16)]
        public float spawnRadius = 100000f;
        public int amount = 1000;
        public float minScale = 1f;
        public float maxScale = 20f;
        public GameObject[] objects = { };

        [Header("Debug")]
        public bool showSpawnRadius = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (seed != 0)
            {
                var previousState = Random.state;
                Random.InitState(seed);
                SpawnObjects();
                Random.state = previousState;
            }
            else
            {
                SpawnObjects();
            }
        }

        private void SpawnObjects()
        {
            if (objects.Length == 0) return;

            for (int i = 0; i < amount; i++)
            {
                InstantiateObject();
            }
        }
        private GameObject InstantiateObject()
        {
            var index = Random.Range(0, objects.Length);
            var instance = Instantiate(objects[index], transform.position + Random.insideUnitSphere * spawnRadius, Random.rotation, transform);
            instance.transform.localScale *= Random.Range(minScale, maxScale);
            return instance;
        }

        private void OnDrawGizmos()
        {
            if (!showSpawnRadius) return;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
