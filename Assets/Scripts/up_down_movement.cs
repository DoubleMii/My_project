using UnityEngine;

public class up_down_movement : MonoBehaviour
{
    [Header("Linear Movement")]
    [SerializeField] private float radius = 3f; //Radius of the circular path.
    [SerializeField] private float speed = 2f; //Rotation speed (higher = faster).
    [SerializeField] private Vector2 centerOffset = Vector2.zero; //Center offset from initial position.
    private Vector2 centerPosition; //Center point of the circular movement.
    private float angle = 0f; //Current rotation angle.

    void Start()
    {
        centerPosition = (Vector2)transform.position + centerOffset; //Set the center position based on initial position plus offset.
    }

    void FixedUpdate()
    {
        angle += speed * Time.fixedDeltaTime; //Increment angle based on speed and time.
        float x = centerPosition.x + Mathf.Cos(angle) * radius; //Calculate X position using cosine.
        float y = centerPosition.y + Mathf.Sin(angle) * radius; //Calculate Y position using sine.
        transform.position = new Vector2(x, y); //Move the platform to the new position.
    }
}
