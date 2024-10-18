using UnityEngine;

namespace SpaceGame
{
    public class PlaneWeapon : MonoBehaviour
    {
        public GameObject projectile;
        public Transform[] projectileSpawnPoints;
        public float projectileLifeTime = 5f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Time.timeScale = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Fire()
        {
            foreach (Transform spawnPoint in projectileSpawnPoints)
            {
                Debug.DrawLine(spawnPoint.position, transform.forward * 10000, Color.red, 1f);
                var laserProjectile = CreateProjectile(projectile, spawnPoint.position, transform.forward.normalized);
                Destroy(laserProjectile, projectileLifeTime);
            }
        }

        private GameObject CreateProjectile(GameObject gameObject, Vector3 position, Vector3 direction)
        {
            GameObject instance = Instantiate(gameObject, position, Quaternion.LookRotation(direction.normalized));
            instance.transform.localScale *= 0.1f;
            return instance;
        }
    }
}
