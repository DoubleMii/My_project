using UnityEngine;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float maxDistance = 5f;

    private Transform player;
    private Vector2 offset;

    void Start()
    {
        player = transform.parent;
        offset = transform.position - player.position;
    }

    void Update()
    {
        Vector2 input = new Vector2(
            (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
            (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));

        if (input.sqrMagnitude > 0)
        {
            offset += input.normalized * moveSpeed * Time.deltaTime;
            offset = Vector2.ClampMagnitude(offset, maxDistance);
        }

        transform.position = player.position + player.TransformVector(offset);

        if (Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("MagneticObjects")) is Collider2D col)
        {
            var mag = col.GetComponent<MagneticObjects>();
            if (Input.GetKey(KeyCode.Q)) mag.SetTarget(transform.position);
            else if (Input.GetKey(KeyCode.E)) mag.SetTarget(transform.position - (col.transform.position - transform.position).normalized * 10f);
            else mag.NoTarget();
        }
    }

    private void OnDrawGizmos()
    {
        if (player) Gizmos.DrawWireSphere(player.position, maxDistance);
    }
}