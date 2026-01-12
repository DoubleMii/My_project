using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCanon : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private List<GameObject> balaPrefabs = new List<GameObject>();
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float cadencia = 1f;
    [SerializeField] private bool shotLeft = false;

    [Header("Teleport Settings")]
    [SerializeField] private Transform[] teleportPositions = new Transform[3]; // Referencias iniciales (pueden ser hijos)
    [SerializeField] private float timeInPosition = 3f;
    [SerializeField] private float shootDelayAfterTeleport = 0.5f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem teleportEffect;
    [SerializeField] private AudioClip teleportSound;

    // Posiciones guardadas como coordenadas absolutas (la clave del fix)
    private Vector3[] cachedPositions;
    private int currentPositionIndex = 0;
    private bool canShoot = false;
    private Coroutine shootingRoutine = null;

    private void Start()
    {
        // Validaciones
        if (teleportPositions == null || teleportPositions.Length == 0)
        {
            Debug.LogError("BossCanon: No hay posiciones de teletransporte asignadas!");
            enabled = false;
            return;
        }

        if (balaPrefabs == null || balaPrefabs.Count == 0)
        {
            Debug.LogError("BossCanon: No hay prefabs de bala asignados!");
            enabled = false;
            return;
        }

        if (puntoDisparo == null)
        {
            Debug.LogError("BossCanon: No se ha asignado el punto de disparo!");
            enabled = false;
            return;
        }

        // Guardamos las posiciones GLOBALES una sola vez (esto soluciona el problema)
        cachedPositions = new Vector3[teleportPositions.Length];
        for (int i = 0; i < teleportPositions.Length; i++)
        {
            if (teleportPositions[i] != null)
            {
                cachedPositions[i] = teleportPositions[i].position;
                // Opcional: puedes ocultar los marcadores visuales
                // teleportPositions[i].gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"BossCanon: La posición de teletransporte {i} está sin asignar!");
                enabled = false;
                return;
            }
        }

        // Teleport inicial a la primera posición
        TeleportToPosition(0);

        // Iniciamos el ciclo
        StartCoroutine(TeleportCycle());
    }

    private IEnumerator TeleportCycle()
    {
        while (true)
        {
            // Tiempo en la posición actual disparando
            yield return new WaitForSeconds(timeInPosition);

            StopShooting();

            PlayTeleportEffect();
            yield return new WaitForSeconds(0.2f); // pequeño efecto visual

            // Siguiente posición (cíclico)
            currentPositionIndex = (currentPositionIndex + 1) % cachedPositions.Length;
            TeleportToPosition(currentPositionIndex);

            PlayTeleportEffect();

            // Pequeña pausa antes de volver a disparar
            yield return new WaitForSeconds(shootDelayAfterTeleport);

            StartShooting();
        }
    }

    private void TeleportToPosition(int index)
    {
        transform.position = cachedPositions[index];
        Debug.Log($"Boss teletransportado a posición {index}: {cachedPositions[index]}");
    }

    private void StartShooting()
    {
        if (shootingRoutine != null)
            StopCoroutine(shootingRoutine);

        canShoot = true;
        shootingRoutine = StartCoroutine(ShootingLoop());
    }

    private void StopShooting()
    {
        canShoot = false;
        if (shootingRoutine != null)
        {
            StopCoroutine(shootingRoutine);
            shootingRoutine = null;
        }
    }

    private IEnumerator ShootingLoop()
    {
        while (canShoot)
        {
            Disparar();
            yield return new WaitForSeconds(cadencia);
        }
    }

    private void Disparar()
    {
        if (!canShoot) return;
        if (balaPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, balaPrefabs.Count);
        GameObject balaPrefab = balaPrefabs[randomIndex];

        if (balaPrefab == null)
        {
            Debug.LogWarning($"Prefab de bala en índice {randomIndex} es null!");
            return;
        }

        Quaternion rotacion = shotLeft ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, -90);

        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, rotacion);

        Bullet bulletScript = bala.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Direccion = shotLeft ? Vector2.left : Vector2.right;
        }
        else
        {
            Debug.LogWarning("La bala instanciada no tiene componente Bullet!");
        }
    }

    private void PlayTeleportEffect()
    {
        if (teleportEffect != null)
        {
            var effect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
            effect.Play();
            float lifetime = effect.main.duration + effect.main.startLifetime.constantMax + 0.5f;
            Destroy(effect.gameObject, lifetime);
        }

        // Descomenta cuando tengas AudioManager
        // if (teleportSound != null && AudioManager.instance != null)
        //     AudioManager.instance.PlaySound(teleportSound);
    }

    // Métodos públicos útiles
    public void ForceStopShooting()
    {
        StopShooting();
        StopAllCoroutines();
    }

    // Limpieza
    private void OnDestroy()
    {
        StopAllCoroutines();
        if (shootingRoutine != null)
            StopCoroutine(shootingRoutine);
    }

    // Opcional: para ver mejor en el editor
    private void OnDrawGizmosSelected()
    {
        if (cachedPositions != null)
        {
            for (int i = 0; i < cachedPositions.Length; i++)
            {
                Gizmos.color = (i == currentPositionIndex) ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(cachedPositions[i], 0.6f);
            }
        }
    }
}