using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickupHeal2D : MonoBehaviour
{
    public string playerTag = "Player";
    public float healAmount = 6f; // ile HP dodaæ

    void Reset()
    {
        // Najwygodniej jako trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        var hp = other.GetComponentInParent<Health>();
        if (hp) hp.Heal(healAmount);

        Destroy(gameObject); // zniknij po podniesieniu
    }

    // Jeœli wolisz zwyk³y collider zamiast triggera, mo¿esz u¿yæ tego:
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag(playerTag)) return;

        var hp = col.collider.GetComponentInParent<Health>();
        if (hp) hp.Heal(healAmount);

        Destroy(gameObject);
    }
}

