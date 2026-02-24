using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzykaWPokoju : MonoBehaviour
{
    public AudioClip x;
    // Start is called before the first frame update
    void Start()
    {
        GameObject okna = GameObject.FindGameObjectWithTag("Player");
        MusicManager menagerOkienek = okna.GetComponent<MusicManager>();
        menagerOkienek.PlaySpecificMusic(x);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
