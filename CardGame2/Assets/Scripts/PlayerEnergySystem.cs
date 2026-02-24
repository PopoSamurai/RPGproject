using UnityEngine;
public class PlayerEnergySystem : MonoBehaviour
{
    public int maxEnergyPerTurn = 3;
    public int currentEnergy;
    void Start()
    {
        StartTurn();
    }
    public void StartTurn()
    {
        currentEnergy = maxEnergyPerTurn;
    }
    public bool HasEnoughEnergy(int cost)
    {
        return currentEnergy >= cost;
    }
    public void UseEnergy(int cost)
    {
        currentEnergy -= cost;
    }
    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
    }
}