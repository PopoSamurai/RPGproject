using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Tactical/Unit")]
public class TacticalUnitData : ScriptableObject
{
    public string displayName = "Unit";
    public Sprite portrait;
    [Min(1)] public int maxHealth = 10;
    [Min(0)] public int strength = 2;
    [Min(1)] public int movementRange = 5;
    [Min(0.1f)] public float moveSpeed = 4f;
    [Min(1)] public int weight = 1;
    public TacticalAttackData basicAttack;
    public TacticalSkillData[] skills;
    public TacticalItemData[] startingItems;
}
