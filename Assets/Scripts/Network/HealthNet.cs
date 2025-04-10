using NUnit.Framework;
using SpaceGame.UI;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth);
            }

            OnHealthChanged += UpdateHealthBar;
            currentHealth.OnValueChanged += OnCurrentHealthChanged;
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
            currentHealth.Value = Mathf.Clamp(currentHealth.Value + amount, 0, maxHealth);
        }

        private void OnCurrentHealthChanged(float oldHealth, float newHealth)
        {
            OnHealthChanged?.Invoke(oldHealth, newHealth);

            if (currentHealth.Value <= 0 && oldHealth > 0)
            {
                OnNoHealth?.Invoke(oldHealth, currentHealth.Value);
            }
        }

        private void UpdateHealthBar(float oldHealth, float newHealth)
        {
            if (healthBar == null) return;
            healthBar.SetHealth(newHealth);
        }
    }
}
