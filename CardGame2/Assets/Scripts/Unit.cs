using System.Collections;
using UnityEngine;
public class Unit : MonoBehaviour
{
    public CardData sourceCard;
    public int maxHP;
    public int currentHP;
    public int attack; 
    public int counter;
    public int baseCounter = 3;
    public bool IsReady => counter <= 0;
    public bool isDead = false;
    public void Tick()
    {
        counter--;

        Debug.Log($"{name} counter = {counter}");
    }
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
        currentHP += amount;
        Debug.Log($"[UNIT] {name} healed {amount}, HP = {currentHP}");
    }
    public void AddAttack(int amount)
    {
        attack += amount;
        Debug.Log("Dodaj atak" + attack);
    }
    public void PerformAttack()
    {
        var card = GetComponent<CardView>();
        if (card == null || card.CurrentSlot == null) return;

        var target = BoardSystem.Instance.GetFrontTarget(card.CurrentSlot);

        if (target != null)
        {
            target.TakeDamage(attack);
            Debug.Log($"{name} attacks {target.name} for {attack}");
        }
    }
    void Die()
    {
        if (isDead) return;
        isDead = true;

        StartCoroutine(DieRoutine());
    }
    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        var card = GetComponent<CardView>();

        if (card != null && card.CurrentSlot != null)
        {
            var slot = card.CurrentSlot;
            slot.line?.HandleUnitDeath(slot.indexInLine);
        }

        Destroy(gameObject);
    }
}