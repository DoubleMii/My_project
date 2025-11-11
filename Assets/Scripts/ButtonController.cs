using UnityEngine;

public class ButtonController : MonoBehaviour
{
    /*[SerializeField] private GameObject door;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        door.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.SetActive(false);
    }*/
  
    [Header("Button Type")]
    [SerializeField] private ButtonType buttonType = ButtonType.OpenDoor; //Type of button action.

    [Header("Target Object")]
    [SerializeField] private GameObject targetObject; //Object that will be affected by the button.

    [Header("Settings")]
    [SerializeField] private bool deactivateButton = true; //Should the button deactivate after use?
    [SerializeField] private bool requirePlayer = true; //Does it require the player to activate?

    //Enum to define button types.
    public enum ButtonType
    {
        OpenDoor,      //Enables the door collider.
        OpenPillar,    //Deactivates pillars or obstacles.
        InvertGravity  //Inverts the player's gravity.
    }

    private void OnTriggerEnter2D(Collider2D collision) //Function called when something enters the button trigger.
    {
        //Check if player is required and if the collision is with the player.
        if (requirePlayer && !collision.CompareTag("Player"))
        {
            return; //Exit if not the player.
        }

        //Execute action based on button type.
        switch (buttonType)
        {
            case ButtonType.OpenDoor:
                OpenDoor(); //Open the door.
                break;

            case ButtonType.OpenPillar:
                OpenPillar(); //Remove the pillar.
                break;

            case ButtonType.InvertGravity:
                InvertGravity(collision); //Invert player's gravity.
                break;
        }

        //Deactivate button if configured to do so.
        if (deactivateButton)
        {
            gameObject.SetActive(false); //Deactivate the button after use.
        }
    }

    private void OpenDoor() //Function to open the door.
    {
        if (targetObject != null)
        {
            CapsuleCollider2D doorCollider = targetObject.GetComponent<CapsuleCollider2D>(); //Get the door collider.
            if (doorCollider != null)
            {
                doorCollider.enabled = true; //Enable the door collider.
            }
            else
            {
                Debug.LogWarning("Target object doesn't have a CapsuleCollider2D component!");
            }
        }
        else
        {
            Debug.LogWarning("No target object assigned to the button!");
        }
    }

    private void OpenPillar() //Function to remove the pillar.
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false); //Deactivate the pillar.
        }
        else
        {
            Debug.LogWarning("No target object assigned to the button!");
        }
    }

    private void InvertGravity(Collider2D collision) //Function to invert player's gravity.
    {
        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>(); //Get player's Rigidbody2D.

        if (playerRb != null)
        {
            playerRb.gravityScale *= -1; //Invert gravity by multiplying by -1.
        }
        else
        {
            Debug.LogWarning("Player doesn't have a Rigidbody2D component!");
        }
    }

}
