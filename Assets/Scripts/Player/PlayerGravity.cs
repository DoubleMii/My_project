using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private PlayerManager manager;
    private bool isGravityInverted = false;
    private int gravityDirection = 1;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
        manager.rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        if (manager == null) return;

        ApplyDynamicGravity();
    }

    private void ApplyDynamicGravity()
    {
        float velocityY = manager.rb.linearVelocity.y * gravityDirection;

        if (velocityY < 0) // Cayendo
        {
            manager.rb.gravityScale = gravityScale * fallMultiplier * gravityDirection;
        }
        else if (velocityY > 0 && !manager.jump.IsJumping()) // Subiendo sin saltar
        {
            manager.rb.gravityScale = gravityScale * lowJumpMultiplier * gravityDirection;
        }
        else // Normal
        {
            manager.rb.gravityScale = gravityScale * gravityDirection;
        }
    }

    public void ToggleGravity()
    {
        isGravityInverted = !isGravityInverted;
        gravityDirection *= -1;

        // Rotar el transform principal
        transform.rotation = Quaternion.Euler(0, 0, isGravityInverted ? 180f : 0f);

        // Rotar los gráficos para mantenerlos correctos
        if (manager.graphicsChild != null)
        {
            manager.graphicsChild.rotation = Quaternion.Euler(0, 0, isGravityInverted ? 180f : 0f);
        }
    }

    public bool IsGravityInverted() => isGravityInverted;
    public int GetGravityDirection() => gravityDirection;
}