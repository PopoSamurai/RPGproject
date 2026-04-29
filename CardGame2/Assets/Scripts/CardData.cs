using UnityEngine;
using UnityEngine.EventSystems;
public enum CardType
{
    Character,
    Attack,
    Heal,
    BuffAttack,
}
[CreateAssetMenu(menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite artwork;
    public CardType type;

    // Character
    public int hp;
    public int attack;
    public int couner;

    // Effects
    public int damageAmount;
    public int healAmount;
    public int buffAttackAmount;
}