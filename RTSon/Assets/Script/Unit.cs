using UnityEngine;
public class Unit : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.allUnitsList.Add(gameObject);
    }
    private void OnDestroy()
    {
        GameManager.instance.allUnitsList.Remove(gameObject);
    }
}