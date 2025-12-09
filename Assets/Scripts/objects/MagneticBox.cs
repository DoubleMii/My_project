using UnityEngine;

public class MagneticObjects : MonoBehaviour
{
    [Header("Object Size")]
    [SerializeField] private ObjectSize objectSize = ObjectSize.Medium;

    [Header("Movement Settings (Auto-configured by size)")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float mass = 1.0f;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    bool hasTarget;
    Vector3 targetPosition;

    // Enum con 5 tamaños diferentes
    public enum ObjectSize
    {
        Tiny,    // Muy pequeño
        Small,   // Pequeño
        Medium,  // Mediano
        Large,   // Grande
        Huge     // Enorme
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (rb == null)
        {
            Debug.LogError("MagneticObjects needs a Rigidbody2D!");
        }

        if (boxCollider == null)
        {
            Debug.LogError("MagneticObjects needs a BoxCollider2D!");
        }

        ConfigureObjectBySize();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && rb != null)
        {
            ConfigureObjectBySize();
        }
    }

    private void ConfigureObjectBySize()
    {
        switch (objectSize)
        {
            case ObjectSize.Tiny:
                speed = 12f;
                mass = 0.3f;
                transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case ObjectSize.Small:
                speed = 10f;
                mass = 0.5f;
                transform.localScale = new Vector3(2f, 2f, 1f);
                break;
            case ObjectSize.Medium:
                speed = 6f;
                mass = 1f;
                transform.localScale = new Vector3(4f, 4f, 1f);
                break;
            case ObjectSize.Large:
                speed = 3f;
                mass = 2f;
                transform.localScale = new Vector3(6f, 6f, 1f);
                break;
            case ObjectSize.Huge:
                speed = 1.5f;
                mass = 4f;
                transform.localScale = new Vector3(8f, 8f, 1f);
                break;
        }

        if (rb != null)
        {
            rb.mass = mass;
            rb.gravityScale = 1f;
        }

        // Ajustar el collider para que el jugador pueda pararse encima
        if (boxCollider != null)
        {
            boxCollider.size = Vector2.one; // Se ajusta automáticamente con el scale
        }
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized;
            float appliedSpeed = speed / Mathf.Sqrt(mass);
            rb.linearVelocity = targetDirection * appliedSpeed;
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void NoTarget()
    {
        hasTarget = false;
    }

    public ObjectSize GetSize()
    {
        return objectSize;
    }

    // ? NUEVO MÉTODO AGREGADO
    public bool IsBeingControlled()
    {
        return hasTarget;
    }
}