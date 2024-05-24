using UnityEngine;
using UnityEngine.UI;
public enum Change
{
    Enemy,
    Stone,
    Wood
}
public class Enemy : MonoBehaviour
{
    public Change change;
    public bool active = true;
    public Text returnTime;
    float waitTime;
    private void Update()
    {
        switch(change)
        {
            case Change.Enemy:
                break;
            case Change.Stone:

                break;
            case Change.Wood:
                break;
        }
    }
}