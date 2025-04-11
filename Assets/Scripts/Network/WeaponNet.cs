using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace SpaceGame
{
    public class WeaponNet : NetworkBehaviour
    {
        public GameObject projectile; // Must be a prefab with NetworkObject attached

        public float projectileLifeTime = 5f;
        public float fireRate = 3f;
        public Transform[] projectileSpawnPoints;
        public AudioClip[] fireSounds;

        private bool canFire = true;
        private AudioSource[] activeSounds;

        private void Awake()
        {
            activeSounds = new AudioSource[projectileSpawnPoints.Length];
        }

        public void Fire(GameObject owner = null)
        {
            if (!canFire) return;
            canFire = false;

            // Only the server spawns projectiles
            if (IsServer)
            {
                FireProjectiles(owner);
            }

            StartCoroutine(FireRateHandler());
        }

        private void FireProjectiles(GameObject owner = null)
        {
            for (var i = 0; i < projectileSpawnPoints.Length; i++)
            {
                var instancedProjectile = CreateProjectile(projectile, projectileSpawnPoints[i], owner);
                PlayFireSound(i, instancedProjectile.transform);

                // Schedule destruction on the server
                StartCoroutine(DestroyAfterTime(instancedProjectile, projectileLifeTime));
            }
        }

        private GameObject CreateProjectile(GameObject gameObject, Transform spawnPoint, GameObject owner = null)
        {
            var headingDirection = Quaternion.FromToRotation(projectile.transform.forward, spawnPoint.forward);

            var instance = Instantiate(gameObject, spawnPoint.position, headingDirection);
            if (instance.TryGetComponent(out NetworkObject netObj))
            {
                netObj.Spawn();
            }
            else
            {
                Debug.LogWarning("Projectile prefab does not have a NetworkObject component.");
            }

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

        IEnumerator DestroyAfterTime(GameObject gameObject, float time = 0f)
        {
            yield return new WaitForSeconds(time);

            if (gameObject.TryGetComponent(out NetworkObject networkObject) && networkObject.IsSpawned)
            {
                networkObject.Despawn(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
