using UnityEngine;

public class PlayerMovement_2 : MonoBehaviour
{
    [SerializeField] private float velocidad = 8f;
    [SerializeField] private float velocidadMaxima = 8f;
    [SerializeField] private float salto = 12f;

    private Rigidbody2D rb;

    public Vector2 moveInput { get; set; }    // Input que viene del PlayerCore
    public bool isGrounded { get; set; }       // Para controlar si puede saltar
    public bool isStunned { get; set; }        // Para controlar si puede moverse

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isStunned)
        {
            return;
        }
        // Aplicar fuerza horizontal solo si no excede la velocidad máxima
        float velX = rb.linearVelocity.x;
        float targetVelX = moveInput.x * velocidadMaxima;
        if (Mathf.Abs(velX) < Mathf.Abs(targetVelX))
        {
            rb.AddForce(new Vector2(moveInput.x * velocidad, 0), ForceMode2D.Force);
        }
    }

    public void Jump()
    {
        if (isGrounded && !isStunned)
        {
            rb.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
}
