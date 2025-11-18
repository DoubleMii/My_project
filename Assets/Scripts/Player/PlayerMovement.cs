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
    [SerializeField] Vector3 objectCheckSize = new Vector4(0.5f, 0.05f); //Size of the box used to detect the object.
    [SerializeField] LayerMask objectLayer; //Layer where the object is.

    [Header("Gravity Inversion")]
    [SerializeField] private float gravityScale = 1f; //Normal gravity scale.
    private bool isGravityInverted = false; //Check if gravity is currently inverted.
    private int gravityDirection = 1; //1 = normal, -1 = inverted.

    Animator animator;
    private bool IsFacingRight = true;
    private ParticleSystem walkParticles;

    void Start()
    {
        playerRigidbody2d = GetComponent<Rigidbody2D>(); //Get the Rigidbody2D component from the player.
        animator = GetComponent<Animator>();
        walkParticles = GetComponentInChildren<ParticleSystem>();
        playerRigidbody2d.gravityScale = gravityScale; //Set initial gravity.
    }

    void Update()
    {
        //Better jump physics for more responsive feel.
        if (playerRigidbody2d.linearVelocityY < 0) //If falling.
        {
            playerRigidbody2d.gravityScale = gravityScale * fallMultiplier * gravityDirection; //Apply fall multiplier.
        }
        else if (playerRigidbody2d.linearVelocityY > 0 && !isJumping) //If moving up but not holding jump.
        {
            playerRigidbody2d.gravityScale = gravityScale * lowJumpMultiplier * gravityDirection; //Apply low jump multiplier.
        }
        else
        {
            playerRigidbody2d.gravityScale = gravityScale * gravityDirection; //Normal gravity.
        }
    }

    void FixedUpdate()
    {
        
        float targetSpeed = playerDirection.x * playerSpeed; //Smooth acceleration and deceleration for better feel.
        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        playerRigidbody2d.linearVelocity = new Vector2(currentSpeed, playerRigidbody2d.linearVelocityY); //Move the player.
        animator.SetFloat("xVel", Mathf.Abs(playerRigidbody2d.linearVelocityX)); //Animation of the player.
        FlipSprite();

        if (IsObject()) //Check if a magnetic object is touching the player.
        {
            StopMagneticObject(); //Stop the magnetic object from being attracted.
        }
    }

    private void FlipSprite()
    {
        if (IsFacingRight && playerDirection.x < 0f || !IsFacingRight && playerDirection.x > 0f)
        {
            IsFacingRight = !IsFacingRight;
            Vector3 playerLocalScale = transform.localScale;
            playerLocalScale.x *= -1f;
            transform.localScale = playerLocalScale;
        }
    }

    public void Move(InputAction.CallbackContext context) //Function to move the player when the movement input is detected.
    {
        playerDirection = context.ReadValue<Vector2>(); //Read the input value and store it in the direction variable.
    }

    public void Jump(InputAction.CallbackContext context) //Function to make the player jump.
    {
        if (context.performed && IsGrounded()) //Check if the jump button is pressed and player is grounded.
        {
            float jumpDirection = isGravityInverted ? -1f : 1f; //Adjust jump direction based on gravity.
            playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocityX, jumpForce * jumpDirection); //Apply the jump force.
            isJumping = true;
        }

        if (context.canceled) //If jump button is released early.
        {
            isJumping = false;
            if ((isGravityInverted && playerRigidbody2d.linearVelocityY < 0) ||
                (!isGravityInverted && playerRigidbody2d.linearVelocityY > 0)) //If moving in jump direction.
            {
                playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocityX, playerRigidbody2d.linearVelocityY * jumpCutMultiplier); //Cut jump short.
            }
        }
    }

    public bool IsGrounded() //Function to check if the player is touching the ground.
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) //Make a box in the ground check position.
        {
            isJumping = false; //Reset jumping flag when grounded.
            return true; //If the box touches the ground layer, return true.
        }
        return false; //If not, return false.
    }

    public bool IsObject() //Function to check if the player is touching a magnetic object.
    {
        if (Physics2D.OverlapBox(objectCheckPos.position, objectCheckSize, 0, objectLayer)) //Make a box in the character to check position.
        {
            return true; //If the box touches the object layer, return true.
        }
        return false; //If not, return false.
    }

    private void StopMagneticObject() //Function to stop the magnetic object that is touching the player.
    {
        Collider2D hitObject = Physics2D.OverlapBox(objectCheckPos.position, objectCheckSize, 0, objectLayer); //Get the collider of the object touching the player.
        MagneticObjects mag = hitObject.GetComponent<MagneticObjects>(); //Get the MagneticObjects script from the object.
        if (mag != null) //If the object has the MagneticObjects script.
        {
            mag.NoTarget(); //Call NoTarget to stop the object's attraction and prevent player floating.
        }
    }

    public void ToggleGravity() //Function to toggle gravity inversion.
    {
        isGravityInverted = !isGravityInverted;
        gravityDirection *= -1; //Flip gravity direction.

        //Flip the player sprite vertically.
        Vector3 playerScale = transform.localScale;
        playerScale.y *= -1f;
        transform.localScale = playerScale;

        //Flip ground check position.
        Vector3 groundCheckLocalPos = groundCheckPos.localPosition;
        groundCheckLocalPos.y *= -1f;
        groundCheckPos.localPosition = groundCheckLocalPos;
    }

    private void OnCollisionEnter2D(Collision2D collision) //Function called when the player collides with something.
    {
        if (collision.gameObject.CompareTag("MovingPlatform")) //If the player lands on a moving platform.
        {
            transform.parent = collision.transform; //Make the player a child of the platform to move with it.
        }
    }

    private void OnCollisionExit2D(Collision2D collision) //Function called when the player stops colliding with something.
    {
        if (collision.gameObject.CompareTag("MovingPlatform")) //If the player leaves the moving platform.
        {
            transform.parent = null; //Unparent the player from the platform.
        }
    }

    public void EmitWalkParticles()
    {
        walkParticles.Emit(1);
    }

    private void OnDrawGizmos() //Function to draw the ground check box and object check box in the editor.
    {
        Gizmos.color = Color.red; //Color of the box.
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize); //Draw the box to visualize the grounded area.
        Gizmos.DrawWireCube(objectCheckPos.position, objectCheckSize); //Draw the box to visualize the object detection area.
    }

    public Vector2 GetDirection()
    {
        return playerDirection;
    }
}