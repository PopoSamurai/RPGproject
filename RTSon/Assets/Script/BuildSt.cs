using UnityEngine;
public class BuildSt : MonoBehaviour
{
    public Sprite icon;
    public string buildName;
    public Sprite skill1Icon;
    void Update()
    {
        if(GameManager.instance.active == false)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void Skill1()
    {
        Debug.Log("Spawn unit");
    }
}