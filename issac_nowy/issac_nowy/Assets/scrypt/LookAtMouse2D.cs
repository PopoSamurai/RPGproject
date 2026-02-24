using UnityEngine;
using UnityEngine.InputSystem; // nowy Input System

[RequireComponent(typeof(Transform))]
public class LookAtMouse2D : MonoBehaviour
{
    [Tooltip("Kamera, z której liczymy pozycjê kursora (zwykle main).")]
    public Camera cam;

    [Tooltip("Prêdkoœæ p³ynnego obrotu (stopnie/sek). 0 = natychmiast.")]
    [Min(0f)] public float rotationSpeed = 0f;

    [Tooltip("Dodatkowy offset k¹ta w stopniach (np. 90 dla sprite’a patrz¹cego w górê).")]
    public float angleOffset = 0f;

    [Tooltip("U¿yj Rigidbody2D.MoveRotation, jeœli masz RB2D (lepsze z fizyk¹).")]
    public bool useRigidbody2D = false;

    Rigidbody2D rb;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (useRigidbody2D) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Pozycja kursora w pikselach ekranu (Input System)
        Vector2 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition; // <-- dla starego Input: wymaga using UnityEngine;

        // Na œwiat 2D
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, Mathf.Abs(cam.transform.position.z)));
        Vector2 dir = (mouseWorld - transform.position);
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;

        if (useRigidbody2D && rb)
        {
            float current = rb.rotation;
            float next = rotationSpeed > 0f
                ? Mathf.MoveTowardsAngle(current, targetAngle, rotationSpeed * Time.deltaTime)
                : targetAngle;
            rb.MoveRotation(next);
        }
        else
        {
            Quaternion desired = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation = rotationSpeed > 0f
                ? Quaternion.RotateTowards(transform.rotation, desired, rotationSpeed * Time.deltaTime)
                : desired;
        }
    }
}

