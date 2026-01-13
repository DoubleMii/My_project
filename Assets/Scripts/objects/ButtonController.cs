using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Button Type")]
    [SerializeField] private ButtonType buttonType = ButtonType.OpenDoor;

    [Header("Target Object")]
    [SerializeField] private GameObject targetObject;

    [Header("Activation Settings")]
    [SerializeField] private bool requirePlayer = true; // ¿Requiere al jugador?
    [SerializeField] private bool canBeActivatedByBoxes = true; // ¿Las cajas pueden activarlo?
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
        // Si es toggleable y ya está activado, no hacer nada
        if (isToggleable && isActivated)
        {
            return;
        }

        bool canActivate = false;

        // Verificar si puede activarse según quién lo toca
        if (collision.CompareTag("Player"))
        {
            canActivate = true; // El player siempre puede activar (si requirePlayer está en true se valida después)
        }
        else if (collision.CompareTag("MagneticObjects") && canBeActivatedByBoxes)
        {
            canActivate = true; // Las cajas magnéticas pueden activar si está permitido
        }

        // Si requirePlayer está activo, SOLO el player puede activar
        if (requirePlayer && !collision.CompareTag("Player"))
        {
            return;
        }

        // Si no puede activar, salir
        if (!canActivate)
        {
            return;
        }

        // Ejecutar acción del botón
        ExecuteButtonAction(collision);

        isActivated = true;

        // Desactivar botón si está configurado y no es toggleable
        if (deactivateButton && !isToggleable)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si es toggleable, permitir reactivación cuando sale
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
        // Dibujar línea hacia el objeto target
        if (targetObject != null && buttonType != ButtonType.InvertGravity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetObject.transform.position);
        }

        // Dibujar el área del botón
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = isActivated ? Color.green : Color.cyan;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
