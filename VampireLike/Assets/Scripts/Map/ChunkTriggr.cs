using UnityEngine;

public class ChunkTriggr : MonoBehaviour
{
    MapGenerator map;
    public GameObject targetMap;
    void Start()
    {
        map = FindObjectOfType<MapGenerator>();
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            map.currentChunk = targetMap;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (map.currentChunk == targetMap)
            {
                map.currentChunk = null;
            }
        }
    }
}