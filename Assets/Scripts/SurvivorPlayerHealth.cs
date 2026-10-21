using UnityEngine;
using System;

public class SurvivorPlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 350f;
    public float currentHealth = 350f;

    public bool IsDead { get; private set; }

    public event Action<float, float> HealthChanged;
    public event Action PlayerDied;

    private void Start()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            IsDead = true;
            PlayerDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(float amount)
    {
        if (amount <= 0f)
            return;

        maxHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        IsDead = false;
        currentHealth = maxHealth;

        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
}