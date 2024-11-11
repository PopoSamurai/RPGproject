using UnityEngine;

public class HealthPotion : MonoBehaviour, Collect
{
    public int healthTorestore;
    public void Collected()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.RestoreHealth(healthTorestore);
        Destroy(gameObject);
    }
}