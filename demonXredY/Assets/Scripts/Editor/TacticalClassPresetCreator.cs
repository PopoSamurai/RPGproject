#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class TacticalClassPresetCreator
{
    [MenuItem("Tools/Tactical/Create 3 Class Presets")]
    public static void Create()
    {
        const string folder = "Assets/TacticalClasses";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "TacticalClasses");
        CreateOne(folder, "Warrior", TacticalBaseClass.Warrior, "Frontline fighter, defense or aggressive melee combat.");
        CreateOne(folder, "Archer", TacticalBaseClass.Archer, "Ranged combat, precision and mobility.");
        CreateOne(folder, "Mage", TacticalBaseClass.Mage, "Spells, battlefield control and support.");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Tactical: 3 class assets ready in " + folder);
    }

    private static void CreateOne(string folder, string name, TacticalBaseClass kind, string description)
    {
        string path = folder + "/" + name + ".asset";
        TacticalClassData data = AssetDatabase.LoadAssetAtPath<TacticalClassData>(path);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<TacticalClassData>();
            data.displayName = name;
            data.baseClass = kind;
            data.description = description;
            AssetDatabase.CreateAsset(data, path);
        }
        // Reconnect existing subclass assets without changing their stats or progression.
        var found = new List<TacticalSubclassData>();
        foreach (string guid in AssetDatabase.FindAssets("t:TacticalSubclassData"))
        {
            var subclass = AssetDatabase.LoadAssetAtPath<TacticalSubclassData>(AssetDatabase.GUIDToAssetPath(guid));
            if (subclass != null && subclass.baseClass == kind) found.Add(subclass);
        }
        found.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        data.availableSubclasses = found.ToArray();
        EditorUtility.SetDirty(data);
    }
}
#endif
