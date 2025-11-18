using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Button Type")]
    [SerializeField] private ButtonType buttonType = ButtonType.OpenDoor; //Type of button action.

    [Header("Target Object")]
    [SerializeField] private GameObject targetObject; //Object that will be affected by the button (not needed for InvertGravity).

    [Header("Settings")]
    [SerializeField] private bool deactivateButton = true; //Should the button deactivate after use?
    [SerializeField] private bool requirePlayer = true; //Does it require the player to activate?
    [SerializeField] private bool isToggleable = false; //Can the button be toggled on/off? (useful for gravity inversion).

    private bool isActivated = false; //Track if button is currently activated (for toggleable buttons).

    //Enum to define button types.
    public enum ButtonType
    {
        OpenDoor,      //Enables the door collider.
        OpenPillar,    //Deactivates pillars or obstacles.
        InvertGravity  //Inverts the player's gravity with visual flip.
    }

    private void OnTriggerEnter2D(Collider2D collision) //Function called when something enters the button trigger.
    {
        //Check if player is required and if the collision is with the player.
        if (requirePlayer && !collision.CompareTag("Player"))
        {
            return; //Exit if not the player.
        }

        //If button is toggleable and already activated, don't activate again.
        if (isToggleable && isActivated)
        {
            return;
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
                InvertGravity(collision); //Invert player's gravity with flip.
                break;
        }

        isActivated = true; //Mark button as activated.

        //Deactivate button if configured to do so and not toggleable.
        if (deactivateButton && !isToggleable)
        {
            gameObject.SetActive(false); //Deactivate the button after use.
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //Function called when something exits the button trigger.
    {
        //If button is toggleable, allow reactivation when player leaves.
        if (isToggleable && requirePlayer && collision.CompareTag("Player"))
        {
            isActivated = false; //Reset activation state.
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

    private void InvertGravity(Collider2D collision) //Function to invert player's gravity with visual flip.
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>(); //Get the PlayerMovement script.

        if (playerMovement != null)
        {
            playerMovement.ToggleGravity(); //Call the ToggleGravity function in PlayerMovement to invert gravity and flip sprite.
            Debug.Log("Player gravity inverted!");
        }
        else
        {
            Debug.LogWarning("Player doesn't have a PlayerMovement component!");
        }
    }
}
