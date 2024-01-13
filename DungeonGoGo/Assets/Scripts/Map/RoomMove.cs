using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMove : MonoBehaviour
{
    public Vector3 playerChange;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && other.isTrigger)
        {
            other.transform.position += playerChange;
        }
    }
}