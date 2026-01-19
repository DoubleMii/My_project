using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] private List<GameObject> balaPrefabs = new List<GameObject>();
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float cadencia = 1f;
    [SerializeField] private bool shotLeft = false;

    private void Start()
    {
        InvokeRepeating(nameof(Disparar), cadencia, cadencia);
    }

    private void Disparar()
    {
        int Rand = Random.Range(0, balaPrefabs.Count);

        // Instanciar la bala
        GameObject balaInstanciada = Instantiate(balaPrefabs[Rand], puntoDisparo.position, shotLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 0));

        // Configurar la dirección ANTES de que empiece a moverse
        Bullet bulletScript = balaInstanciada.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Direccion = shotLeft ? Vector2.left : Vector2.right;
        }
    }
}
