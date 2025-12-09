using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float tiempoMaximo = 5f;
    [SerializeField] private int Damage = 0;
    [SerializeField] private float KnockbackF = 0;
    [SerializeField] private float StunDuration = 0;
    public Vector3 Direccion = Vector2.left;

    private Rigidbody2D rb;
    private MagneticObjects magneticScript;
    private bool hasBeenRepelled = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        magneticScript = GetComponent<MagneticObjects>();
    }

    private void Start()
    {
        // Aplicar velocidad inicial
        rb.linearVelocity = Direccion.normalized * velocidad;

        Invoke(nameof(DestruirBala), tiempoMaximo);
    }

    private void FixedUpdate()
    {
        // Si el sistema magnético está activo, la bala ha sido repelida
        if (magneticScript != null && magneticScript.IsBeingControlled())
        {
            hasBeenRepelled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            // Solo se destruye, el Player decide si muere o no
            DestruirBala();
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

