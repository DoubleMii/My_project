using UnityEngine;
using System.Collections;

public class BossHealthAndStun : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 2.5f;
    [SerializeField] private bool playStunAnimation = true;
    [SerializeField] private string stunAnimationName = "Stun";     

    [Header("Referencias")]
    [SerializeField] private BossCanon bossCanon;                    
    private Animator animator;                                      

    public bool IsStunned { get; private set; } = false;
    public float HealthPercentage => (float)currentHealth / maxHealth;

    public System.Action<int, int> OnHealthChanged;
    public System.Action<bool> OnStunChanged;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (bossCanon == null)
            bossCanon = GetComponent<BossCanon>();

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
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
            return;
        }
        StartCoroutine(DamageFlash());
    }

    public void Stun(float duration = -1f)
    {
        if (IsStunned) return;

        float realDuration = duration > 0 ? duration : stunDuration;

        IsStunned = true;
        OnStunChanged?.Invoke(true);

        //Paramos todo comportamiento normal del boss
        if (bossCanon != null)
        {
            bossCanon.ForceStopShooting();
            bossCanon.StopAllCoroutines();      
        }

        if (playStunAnimation && animator != null)
        {
            animator.Play(stunAnimationName);
        }

        
        StartCoroutine(WaitAndRecover(realDuration));
    }

    private IEnumerator WaitAndRecover(float duration)
    {
        yield return new WaitForSeconds(duration);

        IsStunned = false;
        OnStunChanged?.Invoke(false);

        // Volvemos a la vida normal
        if (bossCanon != null && currentHealth > 0)
        {
        
           bossCanon.StartShooting();
        }
    }

    private IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color original = sr.color;
        sr.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        sr.color = original;
    }

    private void Die()
    {
        IsStunned = true; // para que no siga haciendo nada

        if (bossCanon != null)
        {
            bossCanon.ForceStopShooting();
            bossCanon.StopAllCoroutines();
        }

    }
}
