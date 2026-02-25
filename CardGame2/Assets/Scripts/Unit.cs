using UnityEngine;
public class Unit : MonoBehaviour
{
    public CardData sourceCard;
    public int maxHP;
    public int currentHP;
    public int attack;
    public void Init(CardData data)
    {
        sourceCard = data;
        maxHP = data.hp;
        currentHP = data.hp;
        attack = data.attack;
    }
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"[UNIT] {name} took {dmg} dmg, HP = {currentHP}");

        if (currentHP <= 0)
            Die();
    }
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        Debug.Log($"[UNIT] {name} healed {amount}, HP = {currentHP}");
    }
    public void AddAttack(int amount)
    {
        attack += amount;
        Debug.Log("Dodaj atak" + attack);
    }
    void Die()
    {
        Debug.Log($"[UNIT] {name} died");
        Destroy(gameObject);
    }
}