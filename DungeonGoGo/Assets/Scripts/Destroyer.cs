using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public GameObject poly;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject != poly && other.CompareTag("LevelRoom"))
        {
            Destroy(other.gameObject);
            Debug.Log("zniszczono");
        }
    }
}
