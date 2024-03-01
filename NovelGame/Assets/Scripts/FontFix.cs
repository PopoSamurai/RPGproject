using UnityEngine;
using UnityEngine.UI;
public class FontFix : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = "Abig" + "<color=#CF573C>" + "a" + "</color>" + "il";
    }
}