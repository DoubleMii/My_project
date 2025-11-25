using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    public int CurrentHealth { get; private set; }

    public event Action OnDeath;
    public event Action<int> OnDamage;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        OnDamage?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
    }

    public void ResetHealt()
    {
        CurrentHealth = maxHealth;
    }
}
