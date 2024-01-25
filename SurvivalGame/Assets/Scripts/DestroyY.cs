using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyY : MonoBehaviour
{
    public float life = 3;
    private void Awake()
    {
        Destroy(gameObject,life);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
