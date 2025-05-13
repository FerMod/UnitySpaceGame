using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace SpaceGame.Network
{
    [RequireComponent(typeof(HealthNet))]
    public class RespawnNet : NetworkBehaviour
    {
        public float respawnDelay = 3f;
        private HealthNet health;

        void Start()
        {
            health = GetComponent<HealthNet>();
            health.OnNoHealth += HandleNoHealth;
        }

        private void Update()
        {
            if (IsServer && Input.GetKeyDown(KeyCode.K))
            {
                health.ChangeHealth(-health.MaxHealth);
            }
        }

        new void OnDestroy()
        {
            health.OnNoHealth -= HandleNoHealth;
            base.OnDestroy();
        }

        private void HandleNoHealth(float oldHealth, float newHealth)
        {
            StartCoroutine(RespawnAfterDelay(respawnDelay));
        }

        private IEnumerator RespawnAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            RespawnRpc();
        }

        [Rpc(SendTo.Server)]
        private void RespawnRpc()
        {
            // Reset health to max
            health.ChangeHealth(health.MaxHealth);
            transform.position = Vector3.zero;
        }
    }
}
