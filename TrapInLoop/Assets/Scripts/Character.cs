using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharClass
{ 
    warrior,
    mage,
    archer
}
[CreateAssetMenu(fileName = "New Player", menuName = "Class/Player")]
public class Character : ScriptableObject
{
    public string PlayerName;
    public int classID;
    [Header("Statistics")]
    public int Strength, Luck, Speed, Vitality, Defence, Intelligence, Wisdom, Charisma;
}
