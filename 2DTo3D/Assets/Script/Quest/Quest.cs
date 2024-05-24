using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Quest : MonoBehaviour
{
    public Text questButton;
    public Text questName;
    public Text questDescription;
    public string questTitle;
    public string questDescriptionText;

    public bool complete = false;

    private void Start()
    {
        questButton.text = questTitle;
        questName.text = questTitle;
        questDescription.text = questDescriptionText;
    }
    public void Complete()
    {
        complete = true;
        Debug.Log("get reward");
    }
}
