using UnityEngine;

public class Pause : MonoBehaviour
{
    InputActions p;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p = new InputActions();
        p.UI.Enable();
        p.UI.Pause.performed += ctc =>
        {
            if (Time.tileScale == 0)
            {
                Time.timeScale = 1
                panelPausa.SetActive(false);
            }
            else 
            
           {
                Time.timeScale = 0;
                panelPausa.SetActive(true);

            }
                

        };

        panelPause.SetActive(true);

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
