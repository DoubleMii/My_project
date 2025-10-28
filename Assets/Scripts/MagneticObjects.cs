using UnityEngine;
using UnityEngine.UIElements;

public class MagneticObjects : MonoBehaviour
{
    [SerializeField] float speed = 5.0f; //Speed of the magnetic object.
    Rigidbody2D rb; //Rigidbody to move the object.

    bool hasTarget; //Check if object has a target.
    Vector3 targetPosition; //Position of the target.

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //In Start, assign the Rigidbody of the magnetic object to a variable.
    }

    private void FixedUpdate()
    {
        if (hasTarget) //If the object have a target, it can be attracted by the player.
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized; //Create the target direction by subtracting positions and normalizing it.
            rb.linearVelocity = new Vector2(targetDirection.x, targetDirection.y) * speed; //Multiply direction by speed to move the object.
        }
    }

    public void SetTarget(Vector3 position) //Function to set the target.
    {
        targetPosition = position; //Set target position.
        hasTarget = true; //Enable target.
    }

    public void NoTarget() //Function to unset the target.
    {
        hasTarget = false; //Disable target.
    }

}
