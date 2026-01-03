using UnityEngine;

public class MagneticObjects : MonoBehaviour
{
    [Header("Object Type")]
    [SerializeField] private bool isBullet = false; // Marcar si es una bala

    [Header("Object Size")]
    [SerializeField] private ObjectSize objectSize = ObjectSize.Medium;

    [Header("Movement Settings (Auto-configured by size)")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float mass = 1.0f;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    bool hasTarget;
    Vector3 targetPosition;
    private float originalXPosition;

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
        
        // Solo guardar posición X si NO es bala
        if (!isBullet)
        {
            originalXPosition = transform.position.x;
        }
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
            
            // Si es bala, mantener gravedad en 0, si no, en 1
            rb.gravityScale = isBullet ? 0f : 1f;
            
            // Congelar solo la rotación
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            // Siempre Dynamic
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        // Ajustar el collider
        if (boxCollider != null)
        {
            boxCollider.size = Vector2.one;
        }
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            // Cuando está siendo controlado por el imán
            Vector2 targetDirection = (targetPosition - transform.position).normalized;
            float appliedSpeed = speed / Mathf.Sqrt(mass);
            rb.linearVelocity = targetDirection * appliedSpeed;
            
            // Si es bala y está siendo controlada, activar gravedad
            if (isBullet && rb.gravityScale == 0f)
            {
                rb.gravityScale = 1f;
            }
        }
        else if (!isBullet)
        {
            // Solo para objetos NO bala: mantener posición X fija
            Vector2 currentPos = transform.position;
            transform.position = new Vector2(originalXPosition, currentPos.y);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        // Si es bala sin target, NO tocar su velocidad (dejar que vuele libre)
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void NoTarget()
    {
        hasTarget = false;
        
        if (!isBullet)
        {
            // Solo para objetos NO bala: guardar nueva posición X
            originalXPosition = transform.position.x;
            
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
        // Si es bala, mantener su velocidad actual
    }

    public ObjectSize GetSize()
    {
        return objectSize;
    }

    public bool IsBeingControlled()
    {
        return hasTarget;
    }
}