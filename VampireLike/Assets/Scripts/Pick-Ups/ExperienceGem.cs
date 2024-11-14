using UnityEditor;
using UnityEngine;

public class ExperienceGem : Pickup, Collect
{
    public int experienceGranted;
    public void Collected()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncereaseExperience(experienceGranted);
    }
}