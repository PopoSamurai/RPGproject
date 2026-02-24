using UnityEngine;
using UnityEngine.InputSystem; // nowy Input System

public class SpawnOnRightClickWithCooldown : MonoBehaviour
{
    [Header("Co i gdzie spawnujemy")]
    public GameObject prefab;         // Prefab do zespawnowania
    public Transform spawnPoint;      // Gdzie go postawiæ (pozycja + rotacja)

    [Header("Timingi")]
    [Min(0f)] public float cooldown = 0.5f;  // Odstêp miêdzy strza³ami
    [Min(0f)] public float windup = 0f;      // Opcjonalne opóŸnienie przed strza³em (0 = brak)

    [Header("Zachowanie")]
    public bool queueIfOnCooldown = false;   // Jeœli klikniesz w trakcie cooldownu, wystrzeli po cooldownie

    float nextAllowedTime = 0f;
    bool queuedShot = false;
    bool isWindupRunning = false;

    void Update()
    {
        bool rmbDown = Mouse.current != null
            ? Mouse.current.leftButton.wasPressedThisFrame
            : Input.GetMouseButtonDown(2); // fallback dla starego Input

        if (rmbDown)
        {
            TryFireOrQueue();
        }

        // Jeœli strza³ zosta³ zakolejkowany i min¹³ cooldown – wykonaj
        if (queuedShot && Time.time >= nextAllowedTime && !isWindupRunning)
        {
            queuedShot = false;
            StartCoroutine(FireRoutine());
        }
    }

    void TryFireOrQueue()
    {
        if (Time.time >= nextAllowedTime && !isWindupRunning)
        {
            StartCoroutine(FireRoutine());
        }
        else if (queueIfOnCooldown)
        {
            queuedShot = true;
        }
        // else: ignorujemy klik w cooldownie
    }

    System.Collections.IEnumerator FireRoutine()
    {
        isWindupRunning = true;

        // wind-up: czekamy przed strza³em (jeœli ustawione)
        if (windup > 0f)
            yield return new WaitForSeconds(windup);

        // bezpieczeñstwo: jeœli prefab/spawnPoint nie s¹ ustawione – wyjdŸ
        if (!prefab || !spawnPoint)
        {
            Debug.LogWarning("[SpawnOnRightClickWithCooldown] Brakuje prefab lub spawnPoint.");
            isWindupRunning = false;
            yield break;
        }

        // Spawn
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        // Ustaw nowy czas dozwolonego strza³u
        nextAllowedTime = Time.time + cooldown;
        isWindupRunning = false;
    }
}


