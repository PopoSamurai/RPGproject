using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject player;
    public float speed = 3f;
    public Transform[] points;
    public int i = 0;
    void Update()
    {
        transform.LookAt(player.transform.position);
        transform.Rotate(new Vector3(0, -90, 0), Space.Self);

        if (Vector3.Distance(transform.position, player.transform.position) > 2f)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * speed);
        }

        if (player.GetComponent<Movement>().direction == 0)
        { 
            if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
            {
                i = Random.Range(0, 3);
                if (i == points.Length)
                {
                    i = 0;
                }
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
}
