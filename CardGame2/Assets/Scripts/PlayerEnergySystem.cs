using UnityEngine;
using UnityEngine.UI;
public class PlayerEnergySystem : MonoBehaviour
{
    public int maxEnergyPerTurn = 3;
    public int currentEnergy;
    public Text textEnergy;
    void Start()
    {
        UpdateText();
        StartTurn();
    }
    public void UpdateText()
    {
        textEnergy.text = "Energy: " + currentEnergy;
    }
    public void StartTurn()
    {
        currentEnergy = maxEnergyPerTurn;
        UpdateText();
    }
    public bool HasEnoughEnergy(int cost)
    {
        return currentEnergy >= cost;
    }
    public void UseEnergy(int cost)
    {
        currentEnergy -= cost;
        UpdateText();
    }
    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        UpdateText();
    }
}