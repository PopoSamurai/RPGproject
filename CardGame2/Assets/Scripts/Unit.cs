using UnityEngine;
public class Unit : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
            Die();
    }
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }
    void Die()
    {
        Destroy(gameObject);
    }
}