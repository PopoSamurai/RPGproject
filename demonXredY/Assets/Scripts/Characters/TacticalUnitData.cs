using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Tactical/Unit")]
public class TacticalUnitData : ScriptableObject
{
    public string displayName = "Unit";
    public Sprite portrait;

    [Header("Sprites 2.5D - widok wzgledem postaci")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    [Header("Idle animation (4 frames per direction)")]
    public Sprite[] idleFront = new Sprite[4];
    public Sprite[] idleBack = new Sprite[4];
    public Sprite[] idleLeft = new Sprite[4];
    public Sprite[] idleRight = new Sprite[4];

    [Header("Walk animation (4 frames per direction)")]
    public Sprite[] walkFront = new Sprite[4];
    public Sprite[] walkBack = new Sprite[4];
    public Sprite[] walkLeft = new Sprite[4];
    public Sprite[] walkRight = new Sprite[4];

    [Header("Pickup (2 frames per direction)")]
    public Sprite[] pickupFront = new Sprite[2];
    public Sprite[] pickupBack = new Sprite[2];
    public Sprite[] pickupLeft = new Sprite[2];
    public Sprite[] pickupRight = new Sprite[2];

    [Header("Throw (2 frames per direction)")]
    public Sprite[] throwFront = new Sprite[2];
    public Sprite[] throwBack = new Sprite[2];
    public Sprite[] throwLeft = new Sprite[2];
    public Sprite[] throwRight = new Sprite[2];

    [Header("Carried unit (1 frame per direction)")]
    public Sprite heldFront;
    public Sprite heldBack;
    public Sprite heldLeft;
    public Sprite heldRight;

    [Header("Progression - max level 50")]
    [Range(1, 50)] public int startingLevel = 1;
    public TacticalClassData unitClass;
    public TacticalSubclassData subclass;

    public bool IsSubclassCompatible => unitClass == null || subclass == null || unitClass.Allows(subclass);
[Header("Movement settings (animation/world speed, not a stat)")]
    [Min(0.1f)] public float moveSpeed = 4f;

    [Header("Combat")]
    [Header("Element - unit affinity")]
    public TacticalElement element = TacticalElement.Fire;

    public TacticalAttackData basicAttack;
    public TacticalSkillData[] skills;
    public TacticalItemData[] startingItems;

    public TacticalStats GetStatsAtLevel(int level)
    {
        if (subclass == null || !IsSubclassCompatible)
            Debug.LogError($"{name}: przypisz poprawna podklase z ustawionymi Starting Stats.", this);
        TacticalStats stats = subclass != null && IsSubclassCompatible
            ? subclass.Calculate(level)
            : new TacticalStats();
        stats.hp = Mathf.Max(1, stats.hp);
        stats.mp = Mathf.Max(0, stats.mp);
        stats.movement = Mathf.Max(0, stats.movement);
        stats.jump = Mathf.Max(0, stats.jump);
        stats.jumpDistance = Mathf.Max(0, stats.jumpDistance);
        stats.weight = Mathf.Max(1, stats.weight);
        return stats;
    }

    private void OnValidate()
    {
        startingLevel = Mathf.Clamp(startingLevel, 1, 50);
        if (unitClass != null && subclass != null && !unitClass.Allows(subclass))
            Debug.LogWarning($"{name}: subclass {subclass.name} does not belong to class {unitClass.name}.", this);
    }
}
