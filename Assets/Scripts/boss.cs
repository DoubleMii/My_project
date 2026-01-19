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
    [SerializeField] private Transform[] teleportPositions = new Transform[3];
    [SerializeField] private float timeInPosition = 3f;
    [SerializeField] private float shootDelayAfterTeleport = 0.5f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem teleportEffect;
    [SerializeField] private AudioClip teleportSound;

    private Vector3[] cachedPositions;
    private int currentPositionIndex = 0;
    private bool canShoot = false;
    private Coroutine shootingRoutine = null;

    private void Start()
    {
        if (teleportPositions == null || teleportPositions.Length == 0)
        {
            enabled = false;
            return;
        }

        if (balaPrefabs == null || balaPrefabs.Count == 0)
        {
            enabled = false;
            return;
        }

        if (puntoDisparo == null)
        {
            enabled = false;
            return;
        }

        cachedPositions = new Vector3[teleportPositions.Length];
        for (int i = 0; i < teleportPositions.Length; i++)
        {
            if (teleportPositions[i] != null)
            {
                cachedPositions[i] = teleportPositions[i].position;
            }
            else
            {
                enabled = false;
                return;
            }
        }

        TeleportToPosition(0);
        StartCoroutine(TeleportCycle());
    }

    public IEnumerator TeleportCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeInPosition);
            StopShooting();
            PlayTeleportEffect();
            yield return new WaitForSeconds(0.2f);

            currentPositionIndex = (currentPositionIndex + 1) % cachedPositions.Length;
            TeleportToPosition(currentPositionIndex);

            PlayTeleportEffect();
            yield return new WaitForSeconds(shootDelayAfterTeleport);
            StartShooting();
        }
    }

    private void TeleportToPosition(int index)
    {
        transform.position = cachedPositions[index];
    }

    public void StartShooting()
    {
        if (shootingRoutine != null)
            StopCoroutine(shootingRoutine);

        canShoot = true;
        shootingRoutine = StartCoroutine(ShootingLoop());
    }

    public void StopShooting()
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
        if (balaPrefabs == null || balaPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, balaPrefabs.Count);
        GameObject balaPrefab = balaPrefabs[randomIndex];

        if (balaPrefab == null) return;

        Quaternion rotacion = shotLeft ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, -90);
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, rotacion);

        Bullet bulletScript = bala.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Direccion = shotLeft ? Vector2.left : Vector2.right;
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
    }

    public void ForceStopShooting()
    {
        StopShooting();
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (shootingRoutine != null)
            StopCoroutine(shootingRoutine);
    }

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