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

        Instantiate(balaPrefabs[Rand], puntoDisparo.position, shotLeft ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, -90));

        balaPrefabs[Rand].GetComponent<Bullet>().Direccion = shotLeft ? Vector2.left : Vector2.right;

    }

}
