using SpaceGame.UI;
using System;
using UnityEngine;

namespace SpaceGame
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth = 100f;
        public float MaxHealth => maxHealth;

        [SerializeField]
        private float currentHealth = 100;
        public float CurrentHealth => currentHealth;

        public HealthBar healthBar;

        // Define the health changed event and handler delegate.
        public delegate void HealthChangedHandler(object source, float oldHealth, float newHealth);

        /// <summary>
        /// Invoked when health changes.
        /// </summary>
        public event HealthChangedHandler OnHealthChanged;

        /// <summary>
        /// Invoked when health reaches 0.
        /// </summary>
        public event HealthChangedHandler OnNoHealth;

        private void Start()
        {
            healthBar?.SetMaxHealth(maxHealth);
            OnHealthChanged += UpdateHealthBar;
        }

        public void ChangeHealth(float amount)
        {
            var oldHealth = currentHealth;
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            // Fire health change event.
            OnHealthChanged?.Invoke(this, oldHealth, currentHealth);

            if (currentHealth <= 0)
            {
                OnNoHealth?.Invoke(this, oldHealth, currentHealth);
            }
        }

        private void UpdateHealthBar(object source, float oldHealth, float newHealth)
        {
            healthBar?.SetHealth(newHealth);
        }
    }
}
