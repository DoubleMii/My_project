using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Button Type")]
    [SerializeField] private ButtonType buttonType = ButtonType.OpenDoor;

    [Header("Target Object")]
    [SerializeField] private GameObject targetObject;

    [Header("Activation Settings")]
    [SerializeField] private bool requirePlayer = true; // Requiere al jugador?
    [SerializeField] private bool canBeActivatedByBoxes = true; // Las cajas pueden activarlo?
    [SerializeField] private bool deactivateButton = true;
    [SerializeField] private bool isToggleable = false;

    private bool isActivated = false;

    public enum ButtonType
    {
        OpenDoor,
        OpenPillar,
        InvertGravity
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Button '{gameObject.name}' triggered by '{collision.gameObject.name}' (Tag: {collision.tag})");

        // Si es toggleable y ya esta activado, no hacer nada
        if (isToggleable && isActivated)
        {
            Debug.Log("Button already activated and is toggleable. Ignoring.");
            return;
        }

        bool canActivate = false;

        // Validacion robusta por Componente Y por Tag
        if (collision.GetComponent<PlayerMovement>() != null || collision.CompareTag("Player"))
        {
            canActivate = true;
        }
        else if ((collision.GetComponent<MagneticObjects>() != null || collision.CompareTag("MagneticObjects")) && canBeActivatedByBoxes)
        {
            canActivate = true;
        }

        // Si no puede activar, salir
        if (!canActivate)
        {
            Debug.Log("Object cannot activate this button.");
            return;
        }

        // Ejecutar accion del boton
        ExecuteButtonAction(collision);

        isActivated = true;
        Debug.Log($"Button '{gameObject.name}' ACTIVATED!");

        // Desactivar boton si esta configurado y no es toggleable
        if (deactivateButton && !isToggleable)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si es toggleable, permitir reactivacion cuando sale
        if (isToggleable)
        {
            if ((requirePlayer && collision.CompareTag("Player")) ||
                (canBeActivatedByBoxes && collision.CompareTag("MagneticObjects")))
            {
                isActivated = false;
            }
        }
    }

    private void ExecuteButtonAction(Collider2D collision)
    {
        switch (buttonType)
        {
            case ButtonType.OpenDoor:
                OpenDoor();
                break;

            case ButtonType.OpenPillar:
                OpenPillar();
                break;

            case ButtonType.InvertGravity:
                // Solo el player puede invertir gravedad
                if (collision.CompareTag("Player"))
                {
                    InvertGravity(collision);
                }
                break;
        }
    }

    private void OpenDoor()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("ButtonController: No target object assigned!");
            return;
        }

        CapsuleCollider2D doorCollider = targetObject.GetComponent<CapsuleCollider2D>();
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
            Debug.Log("ButtonController: Door opened!");
        }
        else
        {
            Debug.LogWarning("ButtonController: Target object doesn't have CapsuleCollider2D!");
        }
    }

    private void OpenPillar()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("ButtonController: No target object assigned!");
            return;
        }

        targetObject.SetActive(false);
        Debug.Log("ButtonController: Pillar removed!");
    }

    private void InvertGravity(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.ToggleGravity();
            Debug.Log("ButtonController: Player gravity inverted!");
        }
        else
        {
            Debug.LogWarning("ButtonController: Player doesn't have PlayerMovement component!");
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar linea hacia el objeto target
        if (targetObject != null && buttonType != ButtonType.InvertGravity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetObject.transform.position);
        }

        // Dibujar el area del boton
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = isActivated ? Color.green : Color.cyan;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
