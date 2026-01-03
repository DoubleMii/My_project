using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float tiempoMaximo = 5f;
    [SerializeField] private float tiempoInmunidad = 0.1f;
    public Vector3 Direccion = Vector2.left;

    private Rigidbody2D rb;
    private MagneticObjects magneticScript;
    private bool hasBeenRepelled = false;
    private bool canCollide = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        magneticScript = GetComponent<MagneticObjects>();
        
        // DESACTIVAR MagneticObjects al inicio para que no interfiera
        if (magneticScript != null)
        {
            magneticScript.enabled = false;
        }
    }

    private void Start()
    {
        // FORZAR gravedad a 0
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Direccion.normalized * velocidad;
        }

        // Permitir colisiones después de un momento
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
        // ACTIVAR MagneticObjects solo cuando el imán la detecta
        if (magneticScript != null)
        {
            // Si el script está desactivado pero tiene target, activarlo
            if (!magneticScript.enabled && magneticScript.IsBeingControlled())
            {
                magneticScript.enabled = true;
                hasBeenRepelled = true;
                
                // Activar gravedad
                if (rb != null)
                {
                    rb.gravityScale = 1f;
                }
            }
            // Si está activado y siendo controlado
            else if (magneticScript.enabled && magneticScript.IsBeingControlled())
            {
                hasBeenRepelled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // NO colisionar hasta que pase el tiempo de inmunidad
        if (!canCollide) return;

        // Ignorar colisiones con otras balas
        if (collision.CompareTag("Bullet")) return;

        // Si la bala ha sido repelida y choca con un enemigo
        if (hasBeenRepelled && collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            DestruirBala();
            return;
        }

        // Si choca con el player
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