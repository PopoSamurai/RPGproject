#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TacticalSubclassPresetCreator
{
    [MenuItem("Tools/Tactical/Create 11 Subclass Presets")]
    public static void CreatePresets()
    {
        const string folder = "Assets/TacticalSubclasses";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "TacticalSubclasses");
        // hp, mp, str, def, spd, int, acc, eva PER LEVEL (proposed balancing values)
        Add(folder, "Barbarian", TacticalBaseClass.Warrior, 12,0,2.2f,0.5f,0.8f,0.1f,0.6f,0.5f, TacticalArmorClass.Medium, false, 20,0,1);
        Add(folder, "Guardian", TacticalBaseClass.Warrior, 11,0,1.0f,2.0f,0.3f,0.2f,0.5f,0.2f, TacticalArmorClass.Heavy, true, 25,0,0);
        Add(folder, "Paladin", TacticalBaseClass.Warrior, 9,4,1.1f,1.6f,0.4f,1.1f,0.5f,0.2f, TacticalArmorClass.Heavy, true, 30,0,0);
        Add(folder, "Assassin", TacticalBaseClass.Archer, 7,0,1.3f,0.4f,1.5f,0.2f,1.4f,1.5f, TacticalArmorClass.Light, false, 15,1,0);
        Add(folder, "Artillerist", TacticalBaseClass.Archer, 8,0,1.4f,0.7f,0.5f,0.6f,1.2f,0.3f, TacticalArmorClass.Medium, false, 25,0,0);
        Add(folder, "Marksman", TacticalBaseClass.Archer, 7,0,1.0f,0.5f,1.0f,0.4f,1.7f,0.8f, TacticalArmorClass.Light, false, 20,0,0);
        Add(folder, "Elementalist", TacticalBaseClass.Mage, 6,7,0.2f,0.4f,0.7f,2.3f,0.8f,0.5f, TacticalArmorClass.Light, false, 25,0,0);
        Add(folder, "Druid", TacticalBaseClass.Mage, 8,6,0.6f,0.8f,0.7f,1.7f,0.7f,0.7f, TacticalArmorClass.Medium, false, 25,0,0);
        Add(folder, "Warlock", TacticalBaseClass.Mage, 7,7,0.3f,0.5f,0.7f,2.0f,0.8f,0.6f, TacticalArmorClass.Light, false, 25,0,0);
        Add(folder, "Priest", TacticalBaseClass.Mage, 8,7,0.3f,0.8f,0.5f,1.9f,0.7f,0.4f, TacticalArmorClass.Medium, true, 25,0,0);
        Add(folder, "Summoner", TacticalBaseClass.Mage, 6,8,0.2f,0.4f,0.6f,2.1f,0.7f,0.5f, TacticalArmorClass.Light, false, 25,0,0);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Tactical: created/retained subclass assets in " + folder);
    }

    private static void Add(string folder, string name, TacticalBaseClass kind,
        float hp, float mp, float str, float def, float spd, float intel, float acc, float eva,
        TacticalArmorClass armor, bool shield, int milestone, int jump, int jumpDistance)
    {
        string path = folder + "/" + name + ".asset";
        if (File.Exists(path)) return; // Never overwrite designer-edited assets.
        TacticalSubclassData data = ScriptableObject.CreateInstance<TacticalSubclassData>();
        data.displayName = name;
        data.baseClass = kind;
        data.maximumArmor = armor;
        data.canUseShields = shield;
        data.growth = new TacticalStatGrowth
        {
            hp=hp, mp=mp, str=str, def=def, spd=spd,
            intelligence=intel, accuracy=acc, evasion=eva
        };
        data.levelBonuses = new [] { new TacticalLevelBonus { level=milestone, jump=jump, jumpDistance=jumpDistance } };
        AssetDatabase.CreateAsset(data, path);
    }
}
#endif
