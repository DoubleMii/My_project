using UnityEngine;
public class PlayerAnimation : MonoBehaviour
{
    private PlayerManager manager;
    private ParticleSystem walkParticles;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
        walkParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (manager == null || manager.animator == null) return;

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        // Actualizar velocidad horizontal
        float horizontalSpeed = Mathf.Abs(manager.rb.linearVelocity.x);
        manager.animator.SetFloat("xVel", horizontalSpeed);

        // Actualizar velocidad vertical
        manager.animator.SetFloat("yVel", manager.rb.linearVelocity.y);

        // Actualizar estado de grounded
        manager.animator.SetBool("isGrounded", manager.collision.IsGrounded());

        // Controlar partículas de caminar
        HandleWalkParticles(horizontalSpeed);
    }

    private void HandleWalkParticles(float speed)
    {
        if (walkParticles == null) return;

        bool shouldPlay = speed > 0.1f && manager.collision.IsGrounded();

        if (shouldPlay && !walkParticles.isPlaying)
        {
            walkParticles.Play();
        }
        else if (!shouldPlay && walkParticles.isPlaying)
        {
            walkParticles.Stop();
        }
    }

    public void PlayDeathAnimation()
    {
        if (manager.animator != null)
        {
            manager.animator.SetTrigger("Death");
        }
    }
}