using System.Collections.Generic;
using UnityEngine;

public class DropRateManger : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }
    public List<Drops> drops;
    void OnDestroy()
    {
        float randomNum = UnityEngine.Random.Range(0, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if(randomNum <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
        }
        if (possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            GameObject droppedItem = Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
            Destroy(droppedItem, 5f);
        }
    }
}