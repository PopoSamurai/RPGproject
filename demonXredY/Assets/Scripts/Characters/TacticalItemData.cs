using UnityEngine;

public enum TacticalItemType { Weapon, Armor, Consumable, Accessory }

[CreateAssetMenu(fileName = "NewItem", menuName = "Tactical/Item")]
public class TacticalItemData : ScriptableObject
{
    public string displayName = "Item";
    public Sprite icon;
    [TextArea] public string description;
    public TacticalItemType type;
    [Header("Equipment rules")]
    public TacticalArmorClass armorClass = TacticalArmorClass.None;
    public bool isShield;
    [Min(0)] public int itemWeight;
    [Header("Additional stat modifiers")]
    public int bonusDefense;
    public int bonusMana;
    public int bonusJump;
    public int bonusJumpDistance;
    [Min(0)] public int bonusHealth;
    [Min(0)] public int bonusAttack;
    [Min(0)] public int bonusStrength;
    [Min(0)] public int bonusMovement;
    [Min(0)] public int healAmount;
    public TacticalSkillData grantedSkill;
}
