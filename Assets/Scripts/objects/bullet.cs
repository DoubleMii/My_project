using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float tiempoMaximo = 5f;
    [SerializeField] private float tiempoInmunidad = 0.1f; // Tiempo antes de poder colisionar
    public Vector3 Direccion = Vector2.left;

    private Rigidbody2D rb;
    private MagneticObjects magneticScript;
    private bool hasBeenRepelled = false;
    private bool canCollide = false;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        magneticScript = GetComponent<MagneticObjects>();
        spawnTime = Time.time;

        // ASEGURAR QUE NO TENGA GRAVEDAD AL INICIO
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
    }

    private void Start()
    {
        // Aplicar velocidad inicial INMEDIATAMENTE
        if (rb != null)
        {
            rb.linearVelocity = Direccion.normalized * velocidad;
        }

        // Permitir colisiones después de un breve momento
        Invoke(nameof(EnableCollisions), tiempoInmunidad);
        
        // Destruir después del tiempo máximo
        Invoke(nameof(DestruirBala), tiempoMaximo);
    }

    private void EnableCollisions()
    {
        canCollide = true;
    }

    private void FixedUpdate()
    {
        // Si el sistema magnético está activo, la bala ha sido repelida
        if (magneticScript != null && magneticScript.IsBeingControlled())
        {
            hasBeenRepelled = true;
            
            // Cuando está siendo controlada por el imán, SÍ puede tener gravedad
            if (rb != null)
            {
                rb.gravityScale = 1f;
            }
        }
        else if (!hasBeenRepelled)
        {
            // Si NO está siendo controlada Y NO ha sido repelida, mantener sin gravedad
            if (rb != null && rb.gravityScale != 0f)
            {
                rb.gravityScale = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // NO colisionar hasta que pase el tiempo de inmunidad
        if (!canCollide) return;

        // Ignorar colisiones con otras balas
        if (collision.CompareTag("Bullet")) return;

        // Si la bala ha sido repelida y choca con un enemigo/cañón
        if (hasBeenRepelled && collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            DestruirBala();
            return;
        }

        // Si choca con el player (el PlayerMovement maneja la lógica)
        if (collision.CompareTag("Player"))
        {
            DestruirBala();
            return;
        }

        // Destruir al chocar con paredes/suelo
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            DestruirBala();
        }
    }

    private void DestruirBala()
    {
        CancelInvoke();
        Destroy(gameObject);
    }

    public bool HasBeenRepelled()
    {
        return hasBeenRepelled;
    }
}