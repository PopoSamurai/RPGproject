using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObj characterData;

    //stats
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;

    //Level
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }
    public List<LevelRange> levelsRange;
    //I-Frames
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;
    private void Start()
    {
        experienceCap = levelsRange[0].experienceCapIncrease;
    }
    void Awake()
    {
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
    }
    private void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if(isInvincible)
        {
            isInvincible = false;
        }
    }
    public void IncereaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }

    private void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            //level up
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach(LevelRange range in levelsRange)
            {
                if(level >= range.startLevel&& level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease; 
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
        }
    }
    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            currentHealth -= dmg;

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                kill();
            }
        }
    }

    public void kill()
    {
        Debug.Log("PLAYER DEAD");
    }
    public void RestoreHealth(float amoint)
    {
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += amoint;

            if(currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }
}