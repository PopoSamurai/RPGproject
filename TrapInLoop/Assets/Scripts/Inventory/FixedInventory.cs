using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FixedInventory : ScriptableObject
{
    public bool hasItem;
    public string itemName;
    public int STR, LCK, SPD, VIT, DEF, INT, WIS, CHA;
    public Sprite icon;
}
