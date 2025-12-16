using UnityEngine;

public class PlayerGizmos : MonoBehaviour
{
    private PlayerManager manager;

    [Header("Gizmo Colors")]
    [SerializeField] private Color groundCheckColor = Color.red;
    [SerializeField] private Color objectCheckColor = Color.blue;

    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }

    private void OnDrawGizmos()
    {
        if (manager == null || manager.collision == null) return;

        DrawGroundCheck();
        DrawObjectCheck();
    }

    private void DrawGroundCheck()
    {
        Transform groundPos = manager.collision.GetGroundCheckPos();
        Vector2 groundSize = manager.collision.GetGroundCheckSize();

        if (groundPos != null)
        {
            Gizmos.color = groundCheckColor;
            Gizmos.DrawWireCube(groundPos.position, groundSize);
        }
    }

    private void DrawObjectCheck()
    {
        Transform objectPos = manager.collision.GetObjectCheckPos();
        Vector2 objectSize = manager.collision.GetObjectCheckSize();

        if (objectPos != null)
        {
            Gizmos.color = objectCheckColor;
            Gizmos.DrawWireCube(objectPos.position, objectSize);
        }
    }
}