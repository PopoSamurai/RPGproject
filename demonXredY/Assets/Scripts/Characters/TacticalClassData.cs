using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "Tactical/Class")]
public class TacticalClassData : ScriptableObject
{
    public TacticalBaseClass baseClass;
    public string displayName;
    [TextArea(2, 5)] public string description;
    public Sprite icon;
    [Header("Available specializations")]
    public TacticalSubclassData[] availableSubclasses = new TacticalSubclassData[0];

    public bool Allows(TacticalSubclassData candidate)
    {
        if (candidate == null || candidate.baseClass != baseClass) return false;
        if (availableSubclasses == null || availableSubclasses.Length == 0) return true;
        foreach (var entry in availableSubclasses)
            if (entry == candidate) return true;
        return false;
    }
}
