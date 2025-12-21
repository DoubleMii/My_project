using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Controller : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene("Levels 1_2");

    }

    public void ExitGame()
    {
        Debug.Log("Ha salido del juego");
        Application.Quit();

    }
}