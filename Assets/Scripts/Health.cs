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

        public delegate void HealthChangedHandler(float oldHealth, float newHealth);

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
            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth);
            }

            OnHealthChanged += UpdateHealthBar;
        }

        public void ChangeHealth(float amount)
        {
            var oldHealth = currentHealth;
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (oldHealth != currentHealth)
            {
                OnHealthChanged?.Invoke(oldHealth, currentHealth);
            }

            if (currentHealth <= 0 && oldHealth > 0)
            {
                OnNoHealth?.Invoke(oldHealth, currentHealth);
            }
        }

        private void UpdateHealthBar(float oldHealth, float newHealth)
        {
            if (healthBar == null) return;
            healthBar.SetHealth(newHealth);
        }
    }
}
