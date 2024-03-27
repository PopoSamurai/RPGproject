using System.Collections;
using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    GameObject player;
    public Vector3 offset;
    bool inHouse = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (player.GetComponent<Movement>().posPlayer == 3)
        {
            if (inHouse && player.GetComponent<Movement>().interactOn == true)
            {
                StopFollow();
            }
            else if(player.GetComponent<Movement>().interactOn == true)
            {
                StartCoroutine(waitToTp());
            }
        }
        else
        {
            inHouse = false;
            Follow();
        }
    }

    void Follow()
    {
        Vector3 targetPos = player.transform.position + offset;
        transform.position = targetPos;
    }
    void StopFollow()
    {
        transform.position = this.transform.position;
    }
    IEnumerator waitToTp()
    {
        Follow();
        yield return new WaitForSeconds(1f);
        inHouse = true;
    }    
}