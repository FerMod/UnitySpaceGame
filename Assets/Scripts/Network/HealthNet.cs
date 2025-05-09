using SpaceGame.UI;
using Unity.Netcode;
using UnityEngine;

// https://docs-multiplayer.unity3d.com/netcode/current/basics/networkvariable/#synchronization-and-notification-example
namespace SpaceGame.Network
{
    public class HealthNet : NetworkBehaviour
    {
        [SerializeField]
        private float maxHealth = 100f;
        public float MaxHealth => maxHealth;

        [SerializeField]
        private NetworkVariable<float> currentHealth = new(100f);
        public float CurrentHealth => currentHealth.Value;

        public HealthBar healthBar;

        public delegate void HealthChangedHandler(float oldHealth, float newHealth);

        /// <summary>
        /// Invoked when health changes.
        /// </summary>
        public event HealthChangedHandler OnHealthChanged;

        /// <summary>
        /// Invoked when health reaches 0.
        /// </summary>
        public event HealthChangedHandler OnNoHealth;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                currentHealth.Value = maxHealth;
            }

            currentHealth.OnValueChanged += OnCurrentHealthChanged;

            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth);
            }

            OnHealthChanged += UpdateHealthBar;
        }

        public override void OnNetworkDespawn()
        {
            currentHealth.OnValueChanged -= OnCurrentHealthChanged;
        }

        //private void Start()
        //{
        //    if (healthBar != null)
        //    {
        //        healthBar.SetMaxHealth(maxHealth);
        //    }

        //    OnHealthChanged += UpdateHealthBar;
        //}

        public void ChangeHealth(float amount)
        {
            if (!IsServer) return;
            currentHealth.Value = Mathf.Clamp(currentHealth.Value + amount, 0, maxHealth);
        }

        private void OnCurrentHealthChanged(float oldHealth, float newHealth)
        {
            Debug.Log($"Health changed from {oldHealth} to {newHealth}");
            OnHealthChanged?.Invoke(oldHealth, newHealth);

            if (newHealth <= 0 && oldHealth > 0)
            {
                Debug.Log($"Health reached 0");
                OnNoHealth?.Invoke(oldHealth, newHealth);
            }
        }

        private void UpdateHealthBar(float oldHealth, float newHealth)
        {
            if (healthBar == null) return;
            healthBar.SetHealth(newHealth);
        }
    }
}
