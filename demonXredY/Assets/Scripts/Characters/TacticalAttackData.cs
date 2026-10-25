using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Tactical/Attack")]
public class TacticalAttackData : ScriptableObject
{
    public string displayName = "Basic Attack";
    public Sprite icon;
    [Tooltip("Optional flat power added to STR (or INT for magic). Set 0 for stat-only damage.")]
    [Min(0)] public int damage = 0;
    [Tooltip("Use INT instead of STR for this basic attack.")]
    public bool usesMagic = false;
    [Min(1)] public int range = 1;
    public bool ranged = false;
}
