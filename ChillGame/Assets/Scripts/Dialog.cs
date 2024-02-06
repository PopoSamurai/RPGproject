using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialof/Create New Dialog")]
public class Dialog : ScriptableObject
{
    public Sprite characterSprite, playerEffect;
    public Sprite NPCSprite, npcEffect;
    public string NamePlayer, NameNPC;
    public string message;
    public Dialog nextMessage;
    public int activePlayer = 0;
}