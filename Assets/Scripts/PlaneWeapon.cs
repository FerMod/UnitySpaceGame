using System.Collections;
using UnityEngine;

namespace SpaceGame
{
    public class PlaneWeapon : MonoBehaviour
    {
        public GameObject projectile;

        /// <summary>
        /// Time until the projectile is destroyed.
        /// </summary>
        public float projectileLifeTime = 5f;

        /// <summary>
        /// Projectiles per second.
        /// </summary>
        public float fireRate = 3f;

        public Transform[] projectileSpawnPoints;

        private bool canFire = true;

        public void Fire()
        {
            if (!canFire) return;
            canFire = false;

            FireProjectiles();
            StartCoroutine(FireRateHandler());
        }

        private void FireProjectiles()
        {
            var direction = transform.forward;
            foreach (var spawnPoint in projectileSpawnPoints)
            {
                Debug.DrawLine(spawnPoint.position, direction * 10000, Color.red, 0.5f);
                var laserProjectile = CreateProjectile(projectile, spawnPoint.position, direction);
                Destroy(laserProjectile, projectileLifeTime);
            }
        }

        private GameObject CreateProjectile(GameObject gameObject, Vector3 position, Vector3 direction)
        {
            var instance = Instantiate(gameObject, position, Quaternion.LookRotation(direction));
            return instance;
        }

        IEnumerator FireRateHandler()
        {
            var timeToNextFire = 1 / fireRate;
            yield return new WaitForSecondsRealtime(timeToNextFire);
            canFire = true;
        }

    }
}
