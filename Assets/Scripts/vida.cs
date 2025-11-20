using UnityEngine;

public class vida : MonoBehaviour
{

    Image vidaImage;
    [SerializedField] Health health;
    [SerializedField] int speed = 0.2f;

    void Start()
    {
        vidaImage = GetComoponet<Image>
    }

    
    void Update()
    {
       vidaImage.fillAmount = Mathf.Lerp(vidaImage.fillAmount, (float)health.vidaActual / health.vidaMaxima, speed);
       
    }
}
