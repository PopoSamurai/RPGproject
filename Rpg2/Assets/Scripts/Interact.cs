using UnityEngine;
using UnityEngine.UI;
public enum Change
{
    Enemy,
    Stone,
    Wood
}
public class Interact : MonoBehaviour
{
    public Change change;
    public bool active = false;
    public Text returnTime;
    public float waitTime;
    public string nameOn;
    public BoxCollider2D coll;
    public float timeToEnd;
    public Character enemy;
    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        returnTime.text = nameOn;
        waitTime = timeToEnd;
    }
    private void Update()
    {
        switch (change)
        {
            case Change.Enemy:
                if (active == true)
                {
                    returnTime.text = Mathf.Round(waitTime).ToString();
                    waitTime -= Time.deltaTime;
                    coll.enabled = false;
                }
                break;
            case Change.Stone:
                if(active == true)
                {
                    returnTime.text = Mathf.Round(waitTime).ToString();
                    waitTime -= Time.deltaTime;
                    coll.enabled = false;
                }
                break;
            case Change.Wood:
                break;
        }

        if (waitTime <= 0)
        {
            coll.enabled = true;
            returnTime.text = nameOn;
            active = false;
            waitTime = timeToEnd;
        }
    }
}