using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog/Text")]
public class DialogueSystem : ScriptableObject
{
    public string nameNPC;
    public string[] lines;
}
