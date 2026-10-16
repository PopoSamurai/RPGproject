using UnityEngine;

public enum TacticalItemType { Weapon, Armor, Consumable, Accessory }

[CreateAssetMenu(fileName = "NewItem", menuName = "Tactical/Item")]
public class TacticalItemData : ScriptableObject
{
    public string displayName = "Item";
    public Sprite icon;
    [TextArea] public string description;
    public TacticalItemType type;
    [Min(0)] public int bonusHealth;
    [Min(0)] public int bonusAttack;
    [Min(0)] public int bonusStrength;
    [Min(0)] public int bonusMovement;
    [Min(0)] public int healAmount;
    public TacticalSkillData grantedSkill;
}
