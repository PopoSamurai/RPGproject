using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Spell")]
public class SpellData : ScriptableObject
{
    public string spellName = "Spell Name";
    public string description;
    public int power = 20;
    public bool targetsEnemy = true;   // true – atak, false – np. heal na sojusznika
}
