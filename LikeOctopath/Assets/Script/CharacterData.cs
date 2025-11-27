using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int maxHP = 100;
    public int attackPower = 20;
    public int healPower = 15;
    public int speed = 10;
    public bool isPlayer;

    public Sprite sprite;
    [Header("Elements")]
    public ElementType attackElement = ElementType.None;

    [Tooltip("Elements Vulnerabilities")]
    public ElementType[] elementalWeaknesses;

    [Tooltip("Elements Resist")]
    public ElementType[] elementalResistances;

    [Header("Shield / Break")]
    public int maxShield = 0;

    [Header("Spells")]
    public SpellData[] knownSpells;
}