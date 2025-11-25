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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke(nameof(DestruirBala), tiempoMaximo);
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude < velocidad)
        {
            rb.AddForce(Direccion.normalized * velocidad);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DestruirBala();
            collision.gameObject.GetComponent<PlayerMovement>();
        }
    }

    private void DestruirBala()
    {
        Destroy(gameObject);
    }
}
