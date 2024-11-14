using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObj characterData;

    //stats
    public float currentHealth;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;
    //spawn weapon
    public List<GameObject> spawnedWeapon;
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
        characterData = CharacterSelection.GetData();
        CharacterSelection.instance.DestroySingleton();

        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;

        SpawnWeapon(characterData.StartingWeampon);
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
        
        Recover();
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
    void Recover()
    {
        if(currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;

            if(currentHealth > characterData.MaxHealth)
                currentHealth = characterData.MaxHealth;
        }
    }
    public void SpawnWeapon(GameObject weapon)
    {
        GameObject spawnedWeampon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeampon.transform.SetParent(transform);
        spawnedWeapon.Add(spawnedWeampon);
    }
}