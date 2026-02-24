using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyTriggerZone2D : MonoBehaviour
{
    public enum ZoneType { Explode, Damage }

    public ZoneType type = ZoneType.Explode;
    public EnemyExploder2D owner;

    // Stan tylko dla strefy Damage:
    [HideInInspector] public bool IsPlayerInside;
    [HideInInspector] public Collider2D LastPlayerCollider;
    [HideInInspector] public Health LastPlayerHealthInZone;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        if (!owner) owner = GetComponentInParent<EnemyExploder2D>();
    }

    void Awake()
    {
        if (!owner) owner = GetComponentInParent<EnemyExploder2D>();

        // zarejestruj referencje w ownerze dla wygody
        if (type == ZoneType.Explode) owner.explodeZone = this;
        if (type == ZoneType.Damage) owner.damageZone = this;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!owner) return;

        switch (type)
        {
            case ZoneType.Explode:
                owner.TriggerExplodeEntered(other);
                break;

            case ZoneType.Damage:
                if (other.CompareTag(owner.playerTag))
                {
                    IsPlayerInside = true;
                    LastPlayerCollider = other;
                    LastPlayerHealthInZone = other.GetComponentInParent<Health>();
                }
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (type != ZoneType.Damage) return;
        if (other.CompareTag(owner.playerTag))
        {
            IsPlayerInside = false;
            if (LastPlayerCollider == other)
            {
                LastPlayerCollider = null;
                LastPlayerHealthInZone = null;
            }
        }
    }
}
