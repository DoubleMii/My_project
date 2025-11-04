using UnityEngine;

public class DoorController : MonoBehaviour
{
    
    private void OnTriggerEnter2D(CapsuleCollider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
    }
}