using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "new Dialog")]
public class DialogReader : ScriptableObject
{
    public string NamePlayer;
    public string message;
    public DialogReader nextMessage;
}