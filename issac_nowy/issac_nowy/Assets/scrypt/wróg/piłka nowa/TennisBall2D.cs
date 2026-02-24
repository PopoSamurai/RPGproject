using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TennisBall2D : MonoBehaviour
{
    [Header("Ustawienia pi�ki")]
    public float speed = 8f;                    // pr�dko�� pi�ki
    public Vector2 startDirection = new Vector2(1f, 0.3f); // kierunek pocz�tkowy

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // brak grawitacji, brak obrotu
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        // normalizacja wektora, �eby nie by�o turbo w jednym kierunku
        startDirection = startDirection.normalized;
        rb.linearVelocity = startDirection * speed;
    }

    void FixedUpdate()
    {
        // pilnujemy sta�ej pr�dko�ci
        float currentSpeed = rb.linearVelocity.magnitude;

        // ma�e okno tolerancji, �eby nie trzepa�o wektorem
        if (Mathf.Abs(currentSpeed - speed) > 0.01f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contactCount > 0)
        {
            // normalna powierzchni, w kt�r� waln�a pi�ka
            Vector2 normal = col.contacts[0].normal;

            // odbicie wektora pr�dko�ci wzgl�dem normalnej
            Vector2 newDir = Vector2.Reflect(rb.linearVelocity.normalized, normal);

            rb.linearVelocity = newDir * speed;
        }
    }
}
