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
            if (randomNum <= rate.dropRate)
            {
                Instantiate(rate.itemPrefab, transform.position, Quaternion.identity);
            }
        }
        if (possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}