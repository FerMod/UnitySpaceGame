using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100;
    public float MaxHealthMax => maxHealth;

    [SerializeField]
    private float currentHealth = 100;
    public float CurrentHealth => currentHealth;

    // Define the health changed event and handler delegate.
    public delegate void HealthChangedHandler(object source, float oldHealth, float newHealth);
    public event HealthChangedHandler OnHealthChanged;

    public void ChangeHealth(float amount)
    {
        var oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Fire health change event.
        OnHealthChanged?.Invoke(this, oldHealth, currentHealth);
    }

}


