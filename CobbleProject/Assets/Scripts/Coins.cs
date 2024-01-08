using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Coin", menuName = "Coin/Create New Coin")]
public class Coins : ScriptableObject
{
    public int color;
    public string itemName;
}