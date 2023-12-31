using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BiomeName
{
    Empty, Start, forest, dungeon
}
public class Biome : MonoBehaviour
{
    public BiomeName biom;
    public Color baseColor;
    Image img;    
    //enemy
    public Enemy[] enemies;
    void Start()
    {
        img = GetComponent<Image>();
        img.color = baseColor;
    }

    public void ResetCol()
    {
        img.color = baseColor;
    }
}