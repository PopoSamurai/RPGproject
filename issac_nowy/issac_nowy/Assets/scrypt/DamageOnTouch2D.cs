using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageOnTouch2D : MonoBehaviour
{
    [Header("Kogo raniæ")]
    public string playerTag = "Player";

    [Header("Obra¿enia")]
    public float damage = 1f;
    public bool destroySelfOnHit = false; // np. pocisk znika po trafieniu

    void Reset()
    {
        // Jeœli chcesz dzia³aæ jako trigger, zostaw true; inaczej odznacz w Inspectorze
        GetComponent<Collider2D>().isTrigger = true;
    }

    void TryHit(GameObject other)
    {
        if (!other.CompareTag(playerTag)) return;

        var hp = other.GetComponentInParent<Health>();
        if (!hp) return;

        hp.TakeDamage(damage);

        if (destroySelfOnHit)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other.gameObject);
    void OnCollisionEnter2D(Collision2D col) => TryHit(col.gameObject);
}
