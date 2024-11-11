using UnityEngine;

public class ExperienceGem : MonoBehaviour, Collect
{
    public int experienceGranted;
    public void Collected()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncereaseExperience(experienceGranted);
        Destroy(gameObject);
    }
}