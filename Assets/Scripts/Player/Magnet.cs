using UnityEngine;
using UnityEngine.InputSystem;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float maxDistance = 5f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 0.5f;
    [SerializeField] private LayerMask magneticLayer;

    private Transform player;
    private Vector2 offset;
    private Vector2 magnetDirection; // Dirección del imán (WASD)
    private bool attractActive = false; // Q key
    private bool repelActive = false; // E key

    void Start()
    {
        player = transform.parent;
        if (player == null)
        {
            Debug.LogError("Magnet needs to be child of Player!");
            return;
        }
        offset = transform.position - player.position;
    }

    void Update()
    {
        if (player == null) return;

        // Mover el imán con la dirección recibida del Input System
        if (magnetDirection.sqrMagnitude > 0)
        {
            offset += magnetDirection.normalized * moveSpeed * Time.deltaTime;
            offset = Vector2.ClampMagnitude(offset, maxDistance);
        }

        // Actualizar posición del imán
        transform.position = player.position + player.TransformVector(offset);

        // Detectar objetos magnéticos cercanos
        Collider2D col = Physics2D.OverlapCircle(transform.position, detectionRadius, magneticLayer);

        if (col != null)
        {
            MagneticObjects mag = col.GetComponent<MagneticObjects>();

            if (mag != null)
            {
                if (attractActive) // Q presionada - Atraer
                {
                    mag.SetTarget(transform.position);
                }
                else if (repelActive) // E presionada - Repeler
                {
                    Vector3 repelDirection = (col.transform.position - transform.position).normalized;
                    mag.SetTarget(transform.position - repelDirection * 10f);
                }
                else // Ninguna tecla - Soltar
                {
                    mag.NoTarget();
                }
            }
        }
    }

    // ========== INPUT SYSTEM CALLBACKS ==========

    public void MoveMagnet(InputAction.CallbackContext context)
    {
        magnetDirection = context.ReadValue<Vector2>();
    }

    public void Attract(InputAction.CallbackContext context)
    {
        if (context.performed)
            attractActive = true;

        if (context.canceled)
            attractActive = false;
    }

    public void Repel(InputAction.CallbackContext context)
    {
        if (context.performed)
            repelActive = true;

        if (context.canceled)
            repelActive = false;
    }

    // ========== GIZMOS ==========

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            // Radio máximo del imán
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, maxDistance);

            // Radio de detección
            Gizmos.color = attractActive ? Color.green : (repelActive ? Color.red : Color.cyan);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
