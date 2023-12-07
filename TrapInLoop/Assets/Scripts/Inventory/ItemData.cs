using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public int STR, LCK, SPD, VIT, DEF, INT, WIS, CHA;
    public GameObject model;
    public Sprite icon;
}
