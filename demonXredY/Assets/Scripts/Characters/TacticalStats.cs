using System;
using UnityEngine;

[Serializable]
public struct TacticalStats
{
    [Min(1)] public int hp;
    [Min(0)] public int mp;
    [Min(0)] public int str;
    [Min(0)] public int def;
    [Min(0)] public int spd;
    [Min(0)] public int intelligence;
    [Min(0)] public int accuracy;
    [Min(0)] public int evasion;
    [Min(0)] public int movement;
    [Min(0)] public int jump;
    [Min(0)] public int jumpDistance;
    [Min(0)] public int weight;
    [Min(0)] public int fireRes, iceRes, lightningRes, darkRes, lightRes;
}

[Serializable]
public struct TacticalStatGrowth
{
    public float hp, mp, str, def, spd, intelligence, accuracy, evasion;
    public float fireRes, iceRes, lightningRes, darkRes, lightRes;
}

[Serializable]
public struct TacticalLevelBonus
{
    [Range(2, 50)] public int level;
    public int movement, jump, jumpDistance;
}
