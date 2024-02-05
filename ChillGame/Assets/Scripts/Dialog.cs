using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialof/Create New Dialog")]
public class Dialog : ScriptableObject
{
    public Sprite[] characterSprite;
    public Sprite[] NPCSprite;
    public string NamePlayer, NameNPC;
    public List<string> messages = new List<string>();
}