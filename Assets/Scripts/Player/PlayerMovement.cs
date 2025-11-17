
using UnityEngine;
using UnityEngine.InputSystem;
 
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 2f; //Player speed variable.
    private Rigidbody2D playerRigidbody2d; //Rigidbody of the player to apply forces and movement.
    public Vector2 playerDirection; //Direction the player moves.
 
    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f; //Force used to jump.
 
    [Header("Grounded")]
    [SerializeField] Transform groundCheckPos; //Position from where we check if the player is touching the ground.
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.05f); //Size of the box used to detect the ground.
    [SerializeField] LayerMask groundLayer; //Layer where the ground is.
 
    [Header("ObjectChecker")]
    [SerializeField] Transform objectCheckPos; //Position from where we check if the player is touching the object.
    [SerializeField] Vector3 objectCheckSize = new Vector4(0.5f, 0.05f); //Size of the box used to detect the object.
    [SerializeField] LayerMask objectLayer; //Layer where the object is.

    Animator animator;
    private bool IsFacingRight = true;
    private ParticleSystem walkParticles;
    //AudioManager audiomanagerInstance; //Reference to the AudioManager script to play sounds.
 
    void Start()
    {
        playerRigidbody2d = GetComponent<Rigidbody2D>(); //Get the Rigidbody2D component from the player.
        //audiomanagerInstance = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>(); //Get the AudioManager from the scene.
        animator = GetComponent<Animator>();
        walkParticles = GetComponentInChildren<ParticleSystem>();
    }
 
    void FixedUpdate()
    {
        playerRigidbody2d.linearVelocity = new Vector2(playerDirection.x * playerSpeed, playerRigidbody2d.linearVelocityY); //Move the player by changing his velocity in X.
        animator.SetFloat("xVel",Mathf.Abs(playerRigidbody2d.linearVelocityX)); //Animation of the player  

        FlipSprite();
        
        if (IsObject()) //Check if a magnetic object is touching the player and stop its attraction to prevent floating.
        {
            StopMagneticObject(); //Stop the magnetic object from being attracted.
        }


    }
    private void FlipSprite()
    {
        if(IsFacingRight && playerDirection.x < 0f || !IsFacingRight && playerDirection.x > 0f) 
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
        if (IsGrounded()) //Only can jump if is grounded.
        {
            if (context.performed) //Check if the jump button is pressed.
            {
                playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocityX, jumpForce); //Apply the jump force.
                //audiomanagerInstance.PlaySoundEffect(audiomanagerInstance.jumpSound); //Play the jump sound.
            }
        }
    }
 
    public bool IsGrounded() //Function to check if the player is touching the ground.
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) //Make a box in the ground check position.
        {
            return true; //If the box touchs the ground layer, return true.
        }
        return false; //If not, return false.
    }
 
    public bool IsObject() //Function to check if the player is touching a magnetic object.
    {
        if (Physics2D.OverlapBox(objectCheckPos.position, objectCheckSize, 0, objectLayer)) //Make a box in the character to check position.
        {
            return true; //If the box touchs the object layer, return true.
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

    public Vector2  GetDirection()
    {
        return playerDirection;
    }

}