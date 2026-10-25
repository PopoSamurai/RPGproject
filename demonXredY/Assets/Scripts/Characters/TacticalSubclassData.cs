using UnityEngine;

public enum TacticalBaseClass { Warrior, Archer, Mage }
public enum TacticalArmorClass { None, Light, Medium, Heavy }

[CreateAssetMenu(fileName = "NewSubclass", menuName = "Tactical/Subclass")]
public class TacticalSubclassData : ScriptableObject
{
    public TacticalBaseClass baseClass;
    public string displayName = "Subclass";
    [TextArea] public string roleDescription;
    [Header("Starting Stats - pelne statystyki na poziomie 1")]
    public TacticalStats startingStats;
    [Header("Per-level growth (level 1 is the base stat line)")]
    public TacticalStatGrowth growth;
    [Header("Discrete movement/jump bonuses at specified levels")]
    public TacticalLevelBonus[] levelBonuses = new TacticalLevelBonus[0];
    [Header("Equipment restrictions")]
    public TacticalArmorClass maximumArmor = TacticalArmorClass.Heavy;
    public bool canUseShields = true;
    [Header("Skills unlocked by level (future combat integration)")]
    public TacticalSkillUnlock[] skillUnlocks = new TacticalSkillUnlock[0];

    public TacticalStats Calculate(int level)
    {
        int steps = Mathf.Clamp(level, 1, 50) - 1;
        TacticalStats s = startingStats;
        s.hp = Mathf.Max(1, s.hp + Mathf.RoundToInt(growth.hp * steps));
        s.mp = Mathf.Max(0, s.mp + Mathf.RoundToInt(growth.mp * steps));
        s.str = Mathf.Max(0, s.str + Mathf.RoundToInt(growth.str * steps));
        s.def = Mathf.Max(0, s.def + Mathf.RoundToInt(growth.def * steps));
        s.spd = Mathf.Max(0, s.spd + Mathf.RoundToInt(growth.spd * steps));
        s.intelligence = Mathf.Max(0, s.intelligence + Mathf.RoundToInt(growth.intelligence * steps));
        s.accuracy = Mathf.Max(0, s.accuracy + Mathf.RoundToInt(growth.accuracy * steps));
        s.evasion = Mathf.Max(0, s.evasion + Mathf.RoundToInt(growth.evasion * steps));
        s.fireRes += Mathf.RoundToInt(growth.fireRes * steps);
        s.iceRes += Mathf.RoundToInt(growth.iceRes * steps);
        s.lightningRes += Mathf.RoundToInt(growth.lightningRes * steps);
        s.lightRes += Mathf.RoundToInt(growth.lightRes * steps);
        s.darkRes += Mathf.RoundToInt(growth.darkRes * steps);
        if (levelBonuses != null)
            foreach (TacticalLevelBonus bonus in levelBonuses)
                if (bonus.level >= 2 && bonus.level <= level)
                {
                    s.movement += bonus.movement;
                    s.jump += bonus.jump;
                    s.jumpDistance += bonus.jumpDistance;
                }
        return s;
    }
}

[System.Serializable]
public struct TacticalSkillUnlock
{
    [Range(1, 50)] public int level;
    public TacticalSkillData skill;
}
