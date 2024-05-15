using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialof/Create New Dialog")]
public class DialogPref : ScriptableObject
{
    public Sprite characterSprite;
    public Sprite NPCSprite;
    public string NamePlayer, NameNPC;
    public string message;
    public DialogPref nextMessage;
    public int activePlayer = 0;
}