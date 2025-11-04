using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializedField] private GameObject door;

    
    private void OnTriggerEnter2D(CapsuleCollider2D collision)
    {
        door.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.SetActive(false);
    }
}
