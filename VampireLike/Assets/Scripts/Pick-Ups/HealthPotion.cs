using UnityEngine;

public class HealthPotion : Pickup, Collect
{
    public int healthTorestore;
    public void Collected()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.RestoreHealth(healthTorestore);
    }
}