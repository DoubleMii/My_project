using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Type")]
    [SerializeField] private MovementType movementType = MovementType.Linear; //Type of movement for the platform.

    [Header("Linear Movement Settings")]
    [SerializeField] private Vector2 direction = Vector2.up; //Direction of linear movement (will be normalized).
    [SerializeField] private float distance = 3f; //Distance the platform travels in the chosen direction.
    [SerializeField] private float speed = 2f; //Speed of the platform movement.

    [Header("Circular Movement Settings")]
    [SerializeField] private float radius = 3f; //Radius of the circular path.
    [SerializeField] private float circularSpeed = 2f; //Rotation speed for circular movement.
    [SerializeField] private Vector2 centerOffset = Vector2.zero; //Center offset from initial position.

    private Vector2 startPosition; //Initial position of the platform.
    private Vector2 centerPosition; //Center point for circular movement.
    private float angle = 0f; //Current angle for circular movement.
    private bool movingForward = true; //Direction flag for linear movement.

    //Enum to define movement types.
    public enum MovementType
    {
        Linear,    //Moves in a straight line (up, down, left, right, diagonal).
        Circular   //Moves in a circle.
    }

    void Start()
    {
        startPosition = transform.position; //Store the initial position.
        centerPosition = startPosition + centerOffset; //Calculate center for circular movement.
        direction = direction.normalized; //Normalize the direction vector.
    }

    void FixedUpdate()
    {
        if (movementType == MovementType.Linear) //If the movement type is linear.
        {
            MoveLinear(); //Execute linear movement.
        }
        else if (movementType == MovementType.Circular) //If the movement type is circular.
        {
            MoveCircular(); //Execute circular movement.
        }
    }

    private void MoveLinear() //Function to handle linear movement.
    {
        float step = speed * Time.fixedDeltaTime; //Calculate movement step based on speed.

        if (movingForward) //If moving towards the target position.
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition + direction * distance, step); //Move towards target.

            if (Vector2.Distance(transform.position, startPosition + direction * distance) < 0.01f) //If reached the target.
            {
                movingForward = false; //Change direction.
            }
        }
        else //If moving back to start position.
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, step); //Move back to start.

            if (Vector2.Distance(transform.position, startPosition) < 0.01f) //If reached the start.
            {
                movingForward = true; //Change direction.
            }
        }
    }

    private void MoveCircular() //Function to handle circular movement.
    {
        angle += circularSpeed * Time.fixedDeltaTime; //Increment angle based on speed.

        float x = centerPosition.x + Mathf.Cos(angle) * radius; //Calculate X position using cosine.
        float y = centerPosition.y + Mathf.Sin(angle) * radius; //Calculate Y position using sine.

        transform.position = new Vector2(x, y); //Move the platform to the new position.
    }

    private void OnDrawGizmos() //Draw gizmos to visualize movement in the editor.
    {
        Vector2 start = Application.isPlaying ? startPosition : (Vector2)transform.position; //Get start position.

        if (movementType == MovementType.Linear) //Draw gizmos for linear movement.
        {
            Gizmos.color = Color.green;
            Vector2 normalizedDir = direction.normalized;
            Vector2 endPos = start + normalizedDir * distance;

            Gizmos.DrawLine(start, endPos); //Draw line showing the path.
            Gizmos.DrawWireSphere(start, 0.2f); //Draw start point.
            Gizmos.DrawWireSphere(endPos, 0.2f); //Draw end point.
        }
        else if (movementType == MovementType.Circular) //Draw gizmos for circular movement.
        {
            Vector2 center = Application.isPlaying ? centerPosition : start + centerOffset;

            Gizmos.color = Color.cyan;
            DrawCircle(center, radius, 50); //Draw the circular path.

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, 0.2f); //Draw center point.
        }
    }

    private void DrawCircle(Vector2 center, float radius, int segments) //Helper function to draw circles.
    {
        float angleStep = 360f / segments;
        Vector2 prevPoint = center + new Vector2(radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (angleStep * i);
            Vector2 newPoint = center + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}

