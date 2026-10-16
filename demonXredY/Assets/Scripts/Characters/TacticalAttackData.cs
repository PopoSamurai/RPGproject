using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Tactical/Attack")]
public class TacticalAttackData : ScriptableObject
{
    public string displayName = "Basic Attack";
    public Sprite icon;
    [Min(1)] public int damage = 2;
    [Min(1)] public int range = 1;
    public bool ranged = false;
}
