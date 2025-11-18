using UnityEngine;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Movement")]
    [SerializeField] private float moveSpeed = 5f; //Speed at which the magnet moves.
    [SerializeField] private float maxDistance = 5f; //Maximum distance the magnet can move from the player.

    private Transform playerTransform; //Reference to the player transform.
    private Vector2 magnetOffset; //Current offset from player position.

    void Start()
    {
        playerTransform = transform.parent; //Get the player transform (parent).
        if (playerTransform != null)
        {
            magnetOffset = transform.localPosition; //Store initial local position as offset.
        }
    }

    void Update()
    {
        MoveMagnet(); //Handle magnet movement every frame.
    }

    private void OnTriggerStay2D(Collider2D collision) //The OnTriggerStay function is executed all the time the MagneticObject is inside the Magnet influence area.
    {
        if (collision.gameObject.CompareTag("MagneticObjects"))
        {
            MagneticObjects mag = collision.GetComponent<MagneticObjects>(); //This variable takes the MagneticObjects script to can use his public functions.
            if (mag != null)
            {
                if (Input.GetKey(KeyCode.Q)) //If I press the Q key and the object is inside the magnetic field, it will be attracted to the player.
                {
                    mag.SetTarget(transform.position); //Setting the target to the magnet position.
                }
                else if (Input.GetKey(KeyCode.E)) //If I press the E key and the object is inside the magnetic field, it will be repelled from the player.
                {
                    Vector2 repelDirection = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized; //Calculate direction away from magnet.
                    float repelDistance = 3f; //Distance to repel the object.
                    Vector2 repelTarget = (Vector2)collision.transform.position + repelDirection * repelDistance;
                    mag.SetTarget(repelTarget);
                }
                else
                {
                    mag.NoTarget(); //Unsetting the target when it's not in the magnetic field or I don't press Q or E.
                }
            }
        }
    }

    private void MoveMagnet() //Function to move the magnet with WASD keys.
    {
        Vector2 moveDirection = Vector2.zero;

        //Get input for magnet movement.
        if (Input.GetKey(KeyCode.W)) //Move up.
        {
            moveDirection.y = 1f;
        }
        if (Input.GetKey(KeyCode.S)) //Move down.
        {
            moveDirection.y = -1f;
        }
        if (Input.GetKey(KeyCode.A)) //Move left.
        {
            moveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) //Move right.
        {
            moveDirection.x = 1f;
        }

        //Apply movement to magnet offset.
        if (moveDirection != Vector2.zero)
        {
            moveDirection = moveDirection.normalized; //Normalize to prevent faster diagonal movement.
            magnetOffset += moveDirection * moveSpeed * Time.deltaTime;

            //Clamp the offset to stay within max distance.
            magnetOffset = Vector2.ClampMagnitude(magnetOffset, maxDistance);
        }

        //Update magnet local position (this way it works regardless of player rotation/flip).
        if (playerTransform != null)
        {
            transform.localPosition = magnetOffset; //Use local position to avoid rotation issues.
        }
    }

    private void OnDrawGizmos() //Draw the maximum distance range in the editor.
    {
        if (playerTransform != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f); //Cyan transparent color.
            Gizmos.DrawWireSphere(playerTransform.position, maxDistance); //Draw max range circle.
        }
        else if (transform.parent != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawWireSphere(transform.parent.position, maxDistance);
        }
    }
}
