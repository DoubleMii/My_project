using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private PlayerManager manager;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // AudioManager.instance.PlayerSound(damageSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void Die()
    {
        Debug.Log("Player ha muerto! Reiniciando nivel...");

        // AudioManager.instance.PlayerSound(deathSound);

        // Reproducir animación de muerte si existe
        // manager.animationController.PlayDeathAnimation();

        // Reiniciar el nivel
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}