using UnityEngine;
using UnityEngine.EventSystems;
public enum CardType
{
    Character,
    Attack,
    Heal,
    BuffAttack,
    Energy
}
[CreateAssetMenu(menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite artwork;
    public CardType type;
    public int energyCost;

    // Character
    public int hp;
    public int attack;

    // Effects
    public int damageAmount;
    public int healAmount;
    public int energyAmount;
    public int buffAttackAmount;
}