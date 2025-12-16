using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    private PlayerManager manager;
    private Vector2 moveDirection;
    private float currentSpeed;
    private bool isFacingRight = false;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    private void FixedUpdate()
    {
        if (manager == null) return;

        ApplyMovement();
        HandleFlip();
    }

    private void ApplyMovement()
    {
        float targetSpeed = moveDirection.x * playerSpeed;
        float speedChange = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedChange * Time.fixedDeltaTime);

        manager.rb.linearVelocity = new Vector2(currentSpeed, manager.rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        if (manager.graphicsChild == null) return;

        if ((isFacingRight && moveDirection.x < 0f) || (!isFacingRight && moveDirection.x > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = manager.graphicsChild.localScale;
            scale.x *= -1f;
            manager.graphicsChild.localScale = scale;
        }
    }

    public float GetCurrentSpeed() => currentSpeed;
    public bool IsFacingRight() => isFacingRight;
}