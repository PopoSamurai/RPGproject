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

    [Min(1)] public int maxHealth = 10;
    [Min(0)] public int strength = 2;
    [Min(1)] public int movementRange = 5;
    [Min(0.1f)] public float moveSpeed = 4f;
    [Min(1)] public int weight = 1;
    public TacticalAttackData basicAttack;
    public TacticalSkillData[] skills;
    public TacticalItemData[] startingItems;
}
