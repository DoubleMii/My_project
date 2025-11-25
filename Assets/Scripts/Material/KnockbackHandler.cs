using System;
using System.Collections;
using UnityEngine;

public class KnockbackHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    public event Action<bool> OnStunChanged;
    [SerializeField] private GameObject Body;
    private SpriteRenderer spriteRenderer;
    private bool IsStunned = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = Body.GetComponent<SpriteRenderer>();
    }

    public void ApplyKnockback(Vector2 dir, float force, float stunDuration)
    {
        if (IsStunned)
        {
            return;

        }
        IsStunned = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        if (stunDuration > 0)
        {
            OnStunChanged?.Invoke(true);
            Invoke(nameof(EndStun), stunDuration);
            StartCoroutine(blink(stunDuration));
        }
    }

    private void EndStun()
    {
        OnStunChanged?.Invoke(false);
        IsStunned = false;
    }

    private IEnumerator blink(float stunDuration)
    {
        float TiempoInicio = Time.time;

        while (Time.time < TiempoInicio + stunDuration)

        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);

        }


    }
}
