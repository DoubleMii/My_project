using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Combat")]
    [SerializeField] private float bounceForceMultiplier = 0.6f;

    private PlayerManager manager;
    private bool isJumping = false;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
    }

    public void PerformJump()
    {
        if (!manager.collision.IsGrounded()) return;

        float jumpDirection = manager.gravity.IsGravityInverted() ? -1f : 1f;
        manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, jumpForce * jumpDirection);
        isJumping = true;

        // AudioManager.instance.PlayerSound(jumpSound);
    }

    public void CancelJump()
    {
        isJumping = false;

        bool isGravityInverted = manager.gravity.IsGravityInverted();
        bool isMovingUp = (isGravityInverted && manager.rb.linearVelocity.y < 0) ||
                          (!isGravityInverted && manager.rb.linearVelocity.y > 0);

        if (isMovingUp)
        {
            manager.rb.linearVelocity = new Vector2(
                manager.rb.linearVelocity.x,
                manager.rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    public void ApplyBounce()
    {
        float bounceForce = jumpForce * bounceForceMultiplier;
        float direction = manager.gravity.IsGravityInverted() ? -1f : 1f;

        manager.rb.linearVelocity = new Vector2(manager.rb.linearVelocity.x, bounceForce * direction);

        // AudioManager.instance.PlayerSound(enemyDeathSound);
    }

    public void StopJumping()
    {
        isJumping = false;
    }

    public bool IsJumping() => isJumping;
}