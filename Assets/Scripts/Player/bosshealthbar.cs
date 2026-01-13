using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;      // Arrastra tu Slider aquí
    [SerializeField] private Image fillImage;          // Opcional: para cambiar color por fases

    private BossHealthAndStun bossHealth;

    private void Start()
    {
        // Busca el boss automáticamente (o arrastra manual si prefieres)
        bossHealth = FindObjectOfType<BossHealthAndStun>();
        if (bossHealth == null)
        {
            Debug.LogError("¡No se encontró BossHealthAndStun en la escena!");
            return;
        }

        // Suscribirse al evento de cambio de vida
        bossHealth.OnHealthChanged += UpdateHealthBar;

        // Inicializar la barra al 100%
        UpdateHealthBar(bossHealth.maxHealth, bossHealth.maxHealth);
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Opcional: cambiar color por % de vida
        if (fillImage != null)
        {
            if (currentHealth > maxHealth * 0.6f) fillImage.color = Color.green;
            else if (currentHealth > maxHealth * 0.3f) fillImage.color = Color.yellow;
            else fillImage.color = Color.red;
        }
    }

    private void OnDestroy()
    {
        // Limpieza: desuscribirse para evitar leaks
        if (bossHealth != null)
            bossHealth.OnHealthChanged -= UpdateHealthBar;
    }
}

