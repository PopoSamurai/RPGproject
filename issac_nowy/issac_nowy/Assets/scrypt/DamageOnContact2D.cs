using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageOnContact2D : MonoBehaviour
{
    public enum Target { Player, Enemy }

    [Header("Kogo raniæ? (wybierz w enumie)")]
    public Target target = Target.Player;

    [Header("Tagi")]
    public string playerTag = "Player";
    public string enemyTag = "Enemy";

    [Header("Obra¿enia i zachowanie")]
    public float damage = 10f;
    public bool destroySelfOnHit = false;   // np. pocisk znika po trafieniu
    public bool applyOnTrigger = true;      // reaguj na OnTriggerEnter2D
    public bool applyOnCollision = true;    // reaguj na OnCollisionEnter2D

    // (opcjonalnie) nie raniæ obiektów nale¿¹cych do tego samego "roota" (np. w³asny collider)
    public bool ignoreSameRoot = true;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // najczêœciej wygodniej jako trigger
    }

    string ExpectedTag()
    {
        return target == Target.Player ? playerTag : enemyTag;
    }

    void TryDamage(GameObject other)
    {
        // 1) tylko wybrany cel (enum)
        if (!other.CompareTag(ExpectedTag())) return;

        // 2) (opcjonalnie) nie trafiaj w³asnych dzieci/rodzica
        if (ignoreSameRoot && other.transform.root == transform.root) return;

        // 3) znajdŸ Health na celu (na nim lub u rodzica)
        var hp = other.GetComponentInParent<Health>();
        if (!hp) return;

        hp.TakeDamage(damage);

        if (destroySelfOnHit)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (applyOnTrigger) TryDamage(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (applyOnCollision) TryDamage(collision.gameObject);
    }
}

