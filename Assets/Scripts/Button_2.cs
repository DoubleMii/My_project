using UnityEngine;

public class Button_2 : MonoBehaviour
{
    [SerializeField] private GameObject pillar;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pillar.SetActive (false);
        gameObject.SetActive(false);
    }
}
