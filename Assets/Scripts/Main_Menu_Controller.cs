using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Controller : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene("Levels 1_1");

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}