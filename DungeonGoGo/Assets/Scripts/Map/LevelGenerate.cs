using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerate : MonoBehaviour
{
    public int openDir;
    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;
    public float waitTime = 4f;
    //1 - down
    //2 - top
    //3 - left
    //4 - right
    public void Start()
    {
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }
    void Spawn()
    {
        if (spawned == false)
        {
            if (openDir == 1)
            {
                rand = Random.Range(0, templates.downD.Length);
                Instantiate(templates.downD[rand], transform.position, templates.downD[rand].transform.rotation);
            }
            else if (openDir == 2)
            {
                rand = Random.Range(0, templates.downU.Length);
                Instantiate(templates.downU[rand], transform.position, templates.downU[rand].transform.rotation);
            }
            else if (openDir == 3)
            {
                rand = Random.Range(0, templates.downL.Length);
                Instantiate(templates.downL[rand], transform.position, templates.downL[rand].transform.rotation);
            }
            else if (openDir == 4)
            {
                rand = Random.Range(0, templates.downR.Length);
                Instantiate(templates.downR[rand], transform.position, templates.downR[rand].transform.rotation);
            }
            spawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if(other.GetComponent<LevelGenerate>().spawned == false && spawned == false)
            {
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
                spawned = true;
            }
            spawned = true;
        }
    }
}
