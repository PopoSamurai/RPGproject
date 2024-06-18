using UnityEngine;
public class UnitScr : MonoBehaviour
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