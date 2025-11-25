using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Sound")]
    

    [Header("Animation")]
    [SerializeField] Animator animator;
    private bool IsFacingRight = false;
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
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed,
            (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration) * Time.fixedDeltaTime);

        playerRigidbody2d.linearVelocity = new Vector2(currentSpeed, playerRigidbody2d.linearVelocity.y);

        animator.SetFloat("xVel", Mathf.Abs(playerRigidbody2d.linearVelocity.x));
        FlipSprite();

        if (IsObject())
            StopMagneticObject();
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
            if ((isGravityInverted && playerRigidbody2d.linearVelocity.y < 0) ||
                (!isGravityInverted && playerRigidbody2d.linearVelocity.y > 0))
            {
                playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocity.x,
                    playerRigidbody2d.linearVelocity.y * jumpCutMultiplier);
            }
        }
    }

    // ========== GROUND & OBJECT CHECKS ==========

    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        if (grounded)
            isJumping = false;
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

    // ========== PLATFORM HANDLING ==========

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("MovingPlatform"))
            transform.parent = c.transform;
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("MovingPlatform"))
            transform.parent = null;
    }

    // ========== GIZMOS ==========

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(objectCheckPos.position, objectCheckSize);
    }
}