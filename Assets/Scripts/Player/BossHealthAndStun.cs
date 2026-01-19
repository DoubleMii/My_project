using UnityEngine;
using System.Collections;

public class BossHealthAndStun : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] public int maxHealth = 1000;
    private int currentHealth;

    [Header("Stun por fases")]
    [SerializeField] private float stunDuration = 3.5f;           // duración base de cada stun
    [SerializeField] private int healthThresholdPercent = 20;     // cada 20%
    [SerializeField] private float[] stunMultipliers = { 1f, 1.2f, 1.5f, 1.8f, 2f }; // aumenta duración en fases avanzadas (opcional)

    [Header("Referencias")]
    [SerializeField] private BossCanon bossCanon;

    private int nextThreshold = 80;     // Primer stun cuando baje de 80%
    private int thresholdsHit = 0;

    public bool IsStunned { get; private set; } = false;

    // Evento para que la UI (barra de vida) pueda escuchar los cambios
    public System.Action<int, int> OnHealthChanged;
    private Coroutine currentStunCoroutine;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (bossCanon == null)
        {
            bossCanon = GetComponent<BossCanon>();
        }
    }

    private void Start()
    {
        // Notificamos valor inicial a la UI
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckForPhaseStun()
    {
        float healthPercentage = (float)currentHealth / maxHealth * 100f;

        // Mientras hay umbrales cruzados, aplicamos stun
        while (healthPercentage <= nextThreshold && nextThreshold >= 0)
        {
            TriggerPhaseStun();
            nextThreshold -= healthThresholdPercent;

            // Protección contra bucles infinitos por daño masivo
            if (nextThreshold < -healthThresholdPercent) break;
        }
    }

    private void TriggerPhaseStun()
    {
        thresholdsHit++;

        float currentStunDuration = stunDuration;

        // Aumentamos duración progresivamente (opcional)
        if (thresholdsHit - 1 < stunMultipliers.Length)
        {
            currentStunDuration *= stunMultipliers[thresholdsHit - 1];
        }

        // Check for stun logic

        Stun(currentStunDuration);
    }

    public void Stun(float duration)
    {
        IsStunned = true;

        if (bossCanon != null)
        {
            bossCanon.ForceStopShooting();
        }

        if (currentStunCoroutine != null)
        {
            StopCoroutine(currentStunCoroutine);
        }
        currentStunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr ? sr.color : Color.white;

        if (sr != null)
        {
            sr.color = new Color(1f, 0.4f, 0.4f); // tono rojizo durante stun
        }

        yield return new WaitForSeconds(duration);

        if (sr != null)
        {
            sr.color = originalColor;
        }

        currentStunCoroutine = null;
        RecoverFromStun();
    }

    private void RecoverFromStun()
    {
        IsStunned = false;

        if (currentHealth > 0 && bossCanon != null)
        {
            // Volvemos a iniciar el ciclo completo
            bossCanon.StartCoroutine(bossCanon.TeleportCycle());
            // Si prefieres solo reactivar disparos sin reiniciar teleports:
            // bossCanon.StartShooting();
        }
    }

    private void Die()
    {
        IsStunned = true; // Mark as "dead/inactive"

        if (bossCanon != null)
        {
            bossCanon.StopAllCoroutines();
            bossCanon.ForceStopShooting();
        }

        Destroy(gameObject, 2.5f);
    }

    // Método útil para debug o chequeos externos
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}
