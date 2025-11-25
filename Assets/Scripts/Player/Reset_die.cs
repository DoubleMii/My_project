using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) //Function called when something enters the trigger.
    {
        if (collision.CompareTag("Player")) //If the player enters the trigger.
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reload the current scene.
        }
    }

    private void Update() //Function called every frame.
    {
        if (Input.GetKeyDown(KeyCode.O)) //If the O key is pressed.
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reload the current scene.
        }
    }
}