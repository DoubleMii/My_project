using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    private Rigidbody2D playerRigidbody2d;
    public Vector2 playerDirection;
    private float currentSpeed;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    private bool isJumping = false;

    [Header("Grounded")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] LayerMask groundLayer;

    [Header("ObjectChecker")]
    [SerializeField] Transform objectCheckPos;
    [SerializeField] Vector2 objectCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] LayerMask objectLayer;

    [Header("Gravity Inversion")]
    [SerializeField] private float gravityScale = 1f;
    private bool isGravityInverted = false;
    private int gravityDirection = 1;
    [SerializeField] private Transform graphicsChild;

    [Header("Combat")]
    [SerializeField] private float bounceForceMultiplier = 0.6f;

    [Header("Sound")]

    [Header("Animation")]
    [SerializeField] Animator animator;
    private bool IsFacingRight = true;
    private ParticleSystem walkParticles;

    void Start()
    {
        playerRigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        walkParticles = GetComponentInChildren<ParticleSystem>();
        playerRigidbody2d.gravityScale = gravityScale;
    }

    void Update()
    {
        if (playerRigidbody2d.linearVelocity.y * gravityDirection < 0)
        {
            playerRigidbody2d.gravityScale = gravityScale * fallMultiplier * gravityDirection;
        }
        else if (playerRigidbody2d.linearVelocity.y * gravityDirection > 0 && !isJumping)
        {
            playerRigidbody2d.gravityScale = gravityScale * lowJumpMultiplier * gravityDirection;
        }
        else
        {
            playerRigidbody2d.gravityScale = gravityScale * gravityDirection;
        }
    }

    void FixedUpdate()
    {
        float targetSpeed = playerDirection.x * playerSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration) * Time.fixedDeltaTime);
        playerRigidbody2d.linearVelocity = new Vector2(currentSpeed, playerRigidbody2d.linearVelocity.y);

        animator.SetFloat("xVel", Mathf.Abs(playerRigidbody2d.linearVelocity.x));
        FlipSprite();

        if (IsObject()) StopMagneticObject();
    }

    private void FlipSprite()
    {
        if (IsFacingRight && playerDirection.x < 0f || !IsFacingRight && playerDirection.x > 0f)
        {
            IsFacingRight = !IsFacingRight;
            Vector3 scale = graphicsChild.localScale;
            scale.x *= -1f;
            graphicsChild.localScale = scale;
        }
    }

    // ========== INPUT SYSTEM METHODS ==========
    public void Move(InputAction.CallbackContext context)
    {
        playerDirection = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            float jumpDirection = isGravityInverted ? -1f : 1f;
            playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocity.x, jumpForce * jumpDirection);
            isJumping = true;
            //AudioManager.instance.PlayerSound(jumpSound);
        }

        if (context.canceled)
        {
            isJumping = false;
            if ((isGravityInverted && playerRigidbody2d.linearVelocity.y < 0) || (!isGravityInverted && playerRigidbody2d.linearVelocity.y > 0))
            {
                playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocity.x, playerRigidbody2d.linearVelocity.y * jumpCutMultiplier);
            }
        }
    }

    // ========== GROUND & OBJECT CHECKS ==========
    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        if (grounded) isJumping = false;
        return grounded;
    }

    public bool IsObject()
    {
        return Physics2D.OverlapBox(objectCheckPos.position, objectCheckSize, 0, objectLayer);
    }

    private void StopMagneticObject()
    {
        Collider2D col = Physics2D.OverlapBox(objectCheckPos.position, objectCheckSize, 0, objectLayer);
        col?.GetComponent<MagneticObjects>()?.NoTarget();
    }

    // ========== GRAVITY INVERSION ==========
    public void ToggleGravity()
    {
        isGravityInverted = !isGravityInverted;
        gravityDirection *= -1;
        transform.rotation = Quaternion.Euler(0, 0, isGravityInverted ? 180f : 0f);
        if (graphicsChild != null)
            graphicsChild.rotation = Quaternion.Euler(0, 0, isGravityInverted ? 180f : 0f);
    }

    // ========== COLLISION HANDLING ==========
    // Este método se ejecuta cuando ALGO con Trigger toca AL PLAYER
    // Funciona SOLO si el Player tiene al menos UN collider Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // DEBUG: Ver qué está colisionando
        Debug.Log($"TRIGGER con: {collision.gameObject.name} | Tag: {collision.tag}");
        
        // SOLO procesar balas
        if (collision.CompareTag("Bullet"))
        {
            Debug.Log("¡BALA DETECTADA! Procesando...");
            HandleBulletCollision(collision);
            return;
        }
        
        // Ignorar todo lo demás
        Debug.Log($"Ignorando: {collision.gameObject.name}");
    }

    // Para enemigos y objetos físicos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"COLLISION FISICA con: {collision.gameObject.name} | Tag: {collision.gameObject.tag}");
        
        // Manejar colisiones físicas con enemigos
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision.collider);
        }
        
        // Plataformas móviles
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }

    private void HandleBulletCollision(Collider2D collision)
    {
        // Si es una bala, verificar si ha sido repelida
        Bullet bulletScript = collision.GetComponent<Bullet>();
        bool isSafeBullet = bulletScript != null && bulletScript.HasBeenRepelled();

        // Si la bala ha sido repelida, no hace daño al jugador
        if (isSafeBullet)
        {
            return;
        }

        // El jugador muere
        MorirJugador();
    }

    private void HandleEnemyCollision(Collider2D collision)
    {
        // Calculamos si el jugador está encima del enemigo
        float playerBottom = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
        float enemyTop = collision.transform.position.y + collision.bounds.extents.y;

        // Si el jugador está cayendo y viene desde arriba
        bool isComingFromAbove = playerBottom > enemyTop && playerRigidbody2d.linearVelocity.y * gravityDirection < 0;

        if (isComingFromAbove)
        {
            // Eliminar enemigo
            Destroy(collision.gameObject);

            // Hacer que el jugador rebote
            float bounceForce = jumpForce * bounceForceMultiplier * (isGravityInverted ? -1f : 1f);
            playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocity.x, bounceForce);

            //AudioManager.instance.PlayerSound(enemyDeathSound);
        }
        else
        {
            // El jugador muere
            MorirJugador();
        }
    }

    private void HandleMagneticObjectTrigger(Collider2D collision)
    {
        // Verificar si el jugador viene desde arriba de la caja magnética
        float playerBottom = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
        float boxTop = collision.transform.position.y + collision.bounds.extents.y;

        bool isComingFromAbove = playerBottom > boxTop && playerRigidbody2d.linearVelocity.y * gravityDirection < 0;

        if (isComingFromAbove)
        {
            // Rebotar sobre la caja magnética sin destruirla
            float bounceForce = jumpForce * bounceForceMultiplier * (isGravityInverted ? -1f : 1f);
            playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocity.x, bounceForce);
        }
    }

    private void MorirJugador()
    {
        Debug.Log("Player ha muerto! Reiniciando nivel...");
        //Reproducir animación o sonido de muerte antes de reiniciar
        // AudioManager.instance.PlayerSound(deathSound);
        // Reiniciar el nivel actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ========== GIZMOS ==========
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(objectCheckPos.position, objectCheckSize);
    }
}