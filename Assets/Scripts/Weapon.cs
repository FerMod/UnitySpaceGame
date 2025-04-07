using System.Collections;
using UnityEngine;

namespace SpaceGame
{
    public class Weapon : MonoBehaviour
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

        public AudioClip[] fireSounds;

        private AudioSource[] activeSounds;

        private void Awake()
        {
            activeSounds = new AudioSource[projectileSpawnPoints.Length];
        }

        public void Fire(GameObject owner = null)
        {
            if (!canFire) return;
            canFire = false;

            FireProjectiles(owner);
            StartCoroutine(FireRateHandler());
        }

        private void FireProjectiles(GameObject owner = null)
        {
            for (var i = 0; i < projectileSpawnPoints.Length; i++)
            {
                var instancedProjectile = CreateProjectile(projectile, projectileSpawnPoints[i], owner);
                PlayFireSound(i, instancedProjectile.transform);
                Destroy(instancedProjectile, projectileLifeTime);
            }
            //foreach (var spawnPoint in projectileSpawnPoints)
            //{
            //    var instancedProjectile = CreateProjectile(projectile, spawnPoint);
            //    PlayFireSound(spawnPoint);
            //    Destroy(instancedProjectile, projectileLifeTime);
            //}
        }

        private GameObject CreateProjectile(GameObject gameObject, Transform spawnPoint, GameObject owner = null)
        {
            var headingDirection = Quaternion.FromToRotation(projectile.transform.forward, spawnPoint.forward);

            //Debug.DrawLine(spawnPoint.position, spawnPoint.forward * 10000, Color.red, 0.5f);
            var instance = Instantiate(gameObject, spawnPoint.position, headingDirection);

            if (instance.TryGetComponent(out ProjectileBase projectileBase))
            {
                projectileBase.owner = owner;
            }

            return instance;
        }

        private void PlayFireSound(int index, Transform soundLocation)
        {
            if (activeSounds[index] != null)
            {
                activeSounds[index].Stop();
            }

            if (fireSounds.Length <= 0) return;
            activeSounds[index] = SoundManager.Instance.PlayRandomSoundClip(fireSounds, soundLocation);
        }

        IEnumerator FireRateHandler()
        {
            var timeToNextFire = 1 / fireRate;
            yield return new WaitForSeconds(timeToNextFire);
            canFire = true;
        }

    }
}
