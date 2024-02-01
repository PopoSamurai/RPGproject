using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameM : MonoBehaviour
{
    public Text textRoll;
    public Text textTotal;
    public static int number;
    int score, addS = 0;
    public List<TMP_Text> sides = new List<TMP_Text>();
    public List<int> numberTxt = new List<int>();
    public static bool addBlock = false;
    private void Awake()
    {
        sides.Add(GameObject.Find("TextMeshPro1").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro2").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro3").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro4").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro5").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro6").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro7").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro8").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro9").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro10").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro11").GetComponent<TMP_Text>());
        sides.Add(GameObject.Find("TextMeshPro12").GetComponent<TMP_Text>());

        for (int i = 0; i < Mathf.Min(sides.Count, numberTxt.Count); i++)
        {
            sides[i].text = numberTxt[i].ToString();
        }
    }
    void Update()
    {
        if (!Dice.end)
        {
            AddScore();
            addS = 0;
            textRoll.text = "Result: ?";
        }
        else
        {
            textRoll.text = "Result: " + numberTxt[number].ToString();
            addS = numberTxt[number];
        }

        textTotal.text = "Total: " + score.ToString();
    }
    public void AddScore()
    {
        if (addBlock == false)
        {
            score = addS + score;
            addBlock = true;
        }
    }
}
