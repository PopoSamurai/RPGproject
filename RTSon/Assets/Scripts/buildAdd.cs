using UnityEngine;
public class buildAdd : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.allBuildList.Add(gameObject);
    }
    private void OnDestroy()
    {
        GameManager.instance.allBuildList.Remove(gameObject);
    }
}