using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Magnet : MonoBehaviour
{
    [Header("Magnet Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float maxDistance = 5f;

    [Header("Detection")]
    [SerializeField] private LayerMask magneticLayer;

    [Header("Magnet Position")]
    [SerializeField] GameObject[] mgPos;

    private Transform player;
    private Vector2 offset;
    private bool attractActive = false; // Q key
    private bool repelActive = false;   // E key
    private int magnetCurrDir = 0;

    void Start()
    {
        player = transform.parent;
        if (player == null)
        {
            Debug.LogError("Magnet needs to be child of Player!");
            return;
        }
        offset = transform.position - player.position;

        // Activar la primera posición por defecto (Izquierda)
        if (mgPos != null && mgPos.Length > 0)
        {
            magnetCurrDir = 0;
            for (int i = 0; i < mgPos.Length; i++)
            {
                if (mgPos[i] != null)
                {
                    mgPos[i].SetActive(i == 0);
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();

            if (direction.x > 0) // Derecha
            {
                magnetCurrDir = 1;
                for (int i = 0; i < mgPos.Length; i++)
                {
                    mgPos[i].SetActive(i == magnetCurrDir);
                }
            }
            else if (direction.x < 0) // Izquierda
            {
                magnetCurrDir = 0;
                for (int i = 0; i < mgPos.Length; i++)
                {
                    mgPos[i].SetActive(i == magnetCurrDir);
                }
            }
            else if (direction.y > 0) // Arriba
            {
                magnetCurrDir = 2;
                for (int i = 0; i < mgPos.Length; i++)
                {
                    mgPos[i].SetActive(i == magnetCurrDir);
                }
            }
            else if (direction.y < 0) // Abajo
            {
                magnetCurrDir = 3;
                for (int i = 0; i < mgPos.Length; i++)
                {
                    mgPos[i].SetActive(i == magnetCurrDir);
                }
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // Obtener el collider del imán activo
        GameObject activeMagnet = GetActiveMagnet();
        if (activeMagnet == null) return;

        Collider2D magnetCollider = activeMagnet.GetComponent<Collider2D>();
        if (magnetCollider == null)
        {
            Debug.LogWarning("El GameObject activo no tiene Collider2D!");
            return;
        }

        // Detectar TODOS los objetos magnéticos dentro del collider del imán activo
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(magneticLayer);
        filter.useTriggers = true;

        List<Collider2D> results = new List<Collider2D>();
        int count = Physics2D.OverlapCollider(magnetCollider, filter, results);

        // Aplicar magnetismo a todos los objetos detectados
        for (int i = 0; i < count; i++)
        {
            MagneticObjects mag = results[i].GetComponent<MagneticObjects>();
            if (mag != null)
            {
                Vector3 magnetCenter = magnetCollider.bounds.center;
                Vector3 objectPos = results[i].transform.position;

                if (attractActive) // Q presionada - Atraer
                {
                    mag.SetTarget(magnetCenter);
                }
                else if (repelActive) // E presionada - Repeler
                {
                    Vector3 repelDirection = (objectPos - magnetCenter).normalized;
                    float distance = Vector3.Distance(objectPos, magnetCenter);
                    Vector3 repelTarget = objectPos + repelDirection * (distance + 10f);
                    mag.SetTarget(repelTarget);
                }
                else // Ninguna tecla - Soltar
                {
                    mag.NoTarget();
                }
            }
        }
    }

    // Obtiene el GameObject de imán que está activo
    private GameObject GetActiveMagnet()
    {
        if (mgPos != null && magnetCurrDir >= 0 && magnetCurrDir < mgPos.Length)
        {
            if (mgPos[magnetCurrDir] != null && mgPos[magnetCurrDir].activeSelf)
            {
                return mgPos[magnetCurrDir];
            }
        }
        return null;
    }

    // ========== INPUT SYSTEM CALLBACKS ==========
    public void Attract(InputAction.CallbackContext context)
    {
        if (context.performed) attractActive = true;
        if (context.canceled) attractActive = false;
    }

    public void Repel(InputAction.CallbackContext context)
    {
        if (context.performed) repelActive = true;
        if (context.canceled) repelActive = false;
    }

    // ========== GIZMOS ==========
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            // Radio máximo del imán
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, maxDistance);

            // Dibujar el bounds del collider activo
            GameObject activeMagnet = GetActiveMagnet();
            if (activeMagnet != null)
            {
                Collider2D col = activeMagnet.GetComponent<Collider2D>();
                if (col != null)
                {
                    Gizmos.color = attractActive ? Color.green : (repelActive ? Color.red : Color.cyan);
                    Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                }
            }
        }
    }
}