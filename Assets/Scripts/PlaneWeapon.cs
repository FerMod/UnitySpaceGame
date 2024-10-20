using UnityEngine;

namespace SpaceGame
{
    public class PlaneWeapon : MonoBehaviour
    {
        public GameObject projectile;
        public float projectileLifeTime = 5f;
        public Transform[] projectileSpawnPoints;

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
            var direction = transform.forward;
            foreach (var spawnPoint in projectileSpawnPoints)
            {
                Debug.DrawLine(spawnPoint.position, direction * 10000, Color.red, projectileLifeTime);
                var laserProjectile = CreateProjectile(projectile, spawnPoint.position, direction);
                Destroy(laserProjectile, projectileLifeTime);
            }
        }

        private GameObject CreateProjectile(GameObject gameObject, Vector3 position, Vector3 direction)
        {
            var instance = Instantiate(gameObject, position, Quaternion.LookRotation(direction.normalized));
            instance.transform.localScale *= 0.1f;
            return instance;
        }
    }
}
