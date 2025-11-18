using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 2f; //Player speed variable.
    [SerializeField] private float acceleration = 10f; //How fast the player accelerates.
    [SerializeField] private float deceleration = 10f; //How fast the player decelerates.

    private Rigidbody2D playerRigidbody2d; //Rigidbody of the player to apply forces and movement.
    public Vector2 playerDirection; //Direction the player moves.
    private float currentSpeed; //Current movement speed for smooth acceleration.

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f; //Force used to jump.
    [SerializeField] private float jumpCutMultiplier = 0.5f; //Multiplier when releasing jump early for better control.
    [SerializeField] private float fallMultiplier = 2.5f; //Makes falling feel snappier.
    [SerializeField] private float lowJumpMultiplier = 2f; //Makes small jumps snappier.
    private bool isJumping = false; //Check if player is currently jumping.

    [Header("Grounded")]
    [SerializeField] Transform groundCheckPos; //Position from where we check if the player is touching the ground.
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.05f); //Size of the box used to detect the ground.
    [SerializeField] LayerMask groundLayer; //Layer where the ground is.

    [Header("ObjectChecker")]
    [SerializeField] Transform objectCheckPos; //Position from where we check if the player is touching the object.
    [SerializeField] Vector2 objectCheckSize = new Vector2(0.5f, 0.05f); //Size of the box used to detect the object.
    [SerializeField] LayerMask objectLayer; //Layer where the object is.

    [Header("Gravity Inversion")]
    [SerializeField] private float gravityScale = 1f; //Normal gravity scale.
    private bool isGravityInverted = false; //Check if gravity is currently inverted.
    private int gravityDirection = 1; //1 = normal, -1 = inverted.

    [SerializeField] private Transform graphicsChild;

    Animator animator;
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
        //Better jump physics for more responsive feel.
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
        currentSpeed = Mathf.MoveTowards(currentSpeed,
            targetSpeed,
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
    public void ToggleGravity()
    {
        isGravityInverted = !isGravityInverted;
        gravityDirection *= -1;

        transform.rotation = Quaternion.Euler(0, 0, isGravityInverted ? 180f : 0f);

        if (graphicsChild != null)
            graphicsChild.rotation = Quaternion.Euler(0, 0, isGravityInverted ? 180f : 0f);
    }

    private void OnCollisionEnter2D(Collision2D c) { if (c.gameObject.CompareTag("MovingPlatform")) transform.parent = c.transform; }
    private void OnCollisionExit2D(Collision2D c) { if (c.gameObject.CompareTag("MovingPlatform")) transform.parent = null; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(objectCheckPos.position, objectCheckSize);
    }
}