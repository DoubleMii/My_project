using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInput : MonoBehaviour
{
    private PlayerManager manager;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
    }

    // ========== INPUT SYSTEM CALLBACKS ==========

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        manager.movement.SetMoveDirection(direction);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            manager.jump.PerformJump();
        }

        if (context.canceled)
        {
            manager.jump.CancelJump();
        }
    }

    public void OnToggleGravity(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            manager.gravity.ToggleGravity();
        }
    }
}