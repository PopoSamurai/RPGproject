using UnityEngine;
using UnityEngine.Rendering;
public class SortingLayer : MonoBehaviour
{
    private SortingGroup sortingGroup;
    public float numberOf = -100;

    void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }
    void Update()
    {
        sortingGroup.sortingOrder = Mathf.RoundToInt(transform.position.y * numberOf);
    }
}