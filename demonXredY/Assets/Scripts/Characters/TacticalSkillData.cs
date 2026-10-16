using UnityEngine;

public enum TacticalSkillEffect { Damage, Heal }

[CreateAssetMenu(fileName = "NewSkill", menuName = "Tactical/Skill")]
public class TacticalSkillData : ScriptableObject
{
    public string displayName = "Skill";
    public Sprite icon;
    [TextArea] public string description;
    public TacticalSkillEffect effect = TacticalSkillEffect.Damage;
    [Min(0)] public int power = 3;
    [Min(1)] public int range = 1;
    [Min(0)] public int resourceCost = 0;
}
