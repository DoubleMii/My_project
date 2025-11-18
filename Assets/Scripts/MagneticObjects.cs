using UnityEngine;

public class MagneticObjects : MonoBehaviour
{
    [Header("Object Size")]
    [SerializeField] private ObjectSize objectSize = ObjectSize.Medium; //Size of the magnetic object.

    [Header("Movement Settings (Auto-configured by size)")]
    [SerializeField] private float speed = 5.0f; //Speed of the magnetic object (affected by size).
    [SerializeField] private float mass = 1.0f; //Mass of the object (affected by size).

    Rigidbody2D rb; //Rigidbody to move the object.
    bool hasTarget; //Check if object has a target.
    Vector3 targetPosition; //Position of the target.

    //Enum to define object sizes.
    public enum ObjectSize
    {
        Small,  //Small objects: light and fast.
        Medium, //Medium objects: balanced.
        Large   //Large objects: heavy and slow.
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //In Start, assign the Rigidbody of the magnetic object to a variable.
        if (rb == null)
        {
            Debug.LogError("MagneticObjects needs a Rigidbody2D!");
        }
        ConfigureObjectBySize(); //Configure properties based on selected size.
    }

    private void OnValidate() //Called when values change in the Inspector.
    {
        if (Application.isPlaying && rb != null)
        {
            ConfigureObjectBySize(); //Update properties when size changes in Inspector.
        }
    }

    private void ConfigureObjectBySize() //Function to configure object properties based on size.
    {
        switch (objectSize)
        {
            case ObjectSize.Small:
                speed = 10f; mass = 0.5f;
                transform.localScale = new Vector3(2f, 2f, 1f);
                break;
            case ObjectSize.Medium:
                speed = 6f; mass = 1f;
                transform.localScale = new Vector3(4f, 4f, 1f);
                break;
            case ObjectSize.Large:
                speed = 3f; mass = 2f;
                transform.localScale = new Vector3(6f, 6f, 1f);
                break;
        }

        if (rb != null)
        {
            rb.mass = mass; //Apply mass to Rigidbody2D.
            rb.gravityScale = 1f; //Ensure gravity is always active so it falls when released.
        }
    }

    private void FixedUpdate()
    {
        if (hasTarget) //If the object has a target, it can be attracted by the player.
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized;
            float appliedSpeed = speed / Mathf.Sqrt(mass);
            rb.linearVelocity = targetDirection * appliedSpeed;
        }
    }

    public void SetTarget(Vector3 position) //Function to set the target.
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void NoTarget() //Function to unset the target.
    {
        hasTarget = false;
        //Don't reset velocity to zero - let gravity take over immediately for natural fall.
    }

    public ObjectSize GetSize() //Function to get the current size of the object.
    {
        return objectSize;
    }
}
