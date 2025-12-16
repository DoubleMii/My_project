using UnityEngine;
public class PlayerCollision : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Object Check")]
    [SerializeField] private Transform objectCheckPos;
    [SerializeField] private Vector2 objectCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask objectLayer;

    private PlayerManager manager;

    public void Initialize(PlayerManager playerManager)
    {
        manager = playerManager;
    }

    private void FixedUpdate()
    {
        if (IsObject())
        {
            StopMagneticObject();
        }
    }

    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

        if (grounded && manager.jump != null)
        {
            manager.jump.StopJumping();
        }

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

    // ========== TRIGGER HANDLING ==========
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Bullet"))
        {
            HandleEnemyOrBulletCollision(collision);
        }
    }

    private void HandleEnemyOrBulletCollision(Collider2D collision)
    {
        // Verificar si es una bala repelida
        Bullet bulletScript = collision.GetComponent<Bullet>();
        bool isSafeBullet = bulletScript != null && bulletScript.HasBeenRepelled();

        if (isSafeBullet) return;

        // Verificar si el jugador viene desde arriba
        float playerBottom = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
        float enemyTop = collision.transform.position.y + collision.bounds.extents.y;

        int gravityDir = manager.gravity.GetGravityDirection();
        bool isComingFromAbove = playerBottom > enemyTop && manager.rb.linearVelocity.y * gravityDir < 0;

        if (isComingFromAbove)
        {
            // Destruir enemigo/bala y rebotar
            Destroy(collision.gameObject);
            manager.jump.ApplyBounce();
        }
        else
        {
            // Jugador recibe daño
            manager.health.Die();
        }
    }

    // ========== PLATFORM HANDLING ==========
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }

    // ========== GETTERS FOR GIZMOS ==========
    public Transform GetGroundCheckPos() => groundCheckPos;
    public Vector2 GetGroundCheckSize() => groundCheckSize;
    public Transform GetObjectCheckPos() => objectCheckPos;
    public Vector2 GetObjectCheckSize() => objectCheckSize;
}