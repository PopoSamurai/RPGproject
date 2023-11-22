using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Class/Enemy")]
public class Enemy : ScriptableObject
{
    public string Enemyname;
    [Header("Statistics")]
    public int Strength, Luck, Speed, Vitality, Defence, Intelligence, Wisdom, Charisma;
}
