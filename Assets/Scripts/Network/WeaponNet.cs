using System.Collections;
using UnityEngine;
using Unity.Netcode;
using SpaceGame.Network;

namespace SpaceGame
{
    public class WeaponNet : NetworkBehaviour
    {
        public GameObject projectile;

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
            if (!IsServer) return;
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

                // Schedule destruction on the server
                StartCoroutine(DestroyAfterTime(instancedProjectile, projectileLifeTime));
            }
        }

        private GameObject CreateProjectile(GameObject projectile, Transform spawnPoint, GameObject owner = null)
        {
            var headingDirection = Quaternion.FromToRotation(this.projectile.transform.forward, spawnPoint.forward);

            var instance = Instantiate(projectile, spawnPoint.position, headingDirection);
            IgnoreColliders(owner, instance);

            if (instance.TryGetComponent(out NetworkObject netObj))
            {
                netObj.Spawn();

                if (owner != null && owner.TryGetComponent(out NetworkObject ownerNetObj))
                {
                    IgnoreOwnerCollisionRpc(ownerNetObj.OwnerClientId, netObj);
                }
            }
            else
            {
                Debug.LogWarning("[WeaponNet] ProjectileNet prefab does not have a NetworkObject component.");
            }

            if (instance.TryGetComponent(out ProjectileBaseNet projectileBase))
            {
                projectileBase.owner = owner;
            }

            return instance;
        }

        [Rpc(SendTo.NotServer)]
        private void IgnoreOwnerCollisionRpc(ulong ownerClientId, NetworkObjectReference projectileRef)
        {
            if (NetworkManager.Singleton.LocalClientId != ownerClientId) return;
            if (!projectileRef.TryGet(out var projectile)) return;
            var owner = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()?.gameObject;
            IgnoreColliders(owner, projectile.gameObject);
        }

        private void IgnoreColliders(GameObject owner, GameObject projectile)
        {
            if (owner == null || projectile == null) return;

            var ownerColliders = owner.GetComponentsInChildren<Collider>(includeInactive: true);
            var projectileColliders = projectile.GetComponentsInChildren<Collider>(includeInactive: true);

            if (ownerColliders.Length == 0 || projectileColliders.Length == 0) return;

            foreach (var projectileCollider in projectileColliders)
            {
                foreach (var ownerCollider in ownerColliders)
                {

                    Physics.IgnoreCollision(projectileCollider, ownerCollider);
                }
            }
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

        private IEnumerator FireRateHandler()
        {
            var timeToNextFire = 1 / fireRate;
            yield return new WaitForSeconds(timeToNextFire);
            canFire = true;
        }

        private IEnumerator DestroyAfterTime(GameObject gameObject, float time = 0f)
        {
            yield return new WaitForSeconds(time);
            if (gameObject == null) yield break;
            if (gameObject.TryGetComponent(out NetworkObject networkObject) && networkObject.IsSpawned)
            {
                networkObject.Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
