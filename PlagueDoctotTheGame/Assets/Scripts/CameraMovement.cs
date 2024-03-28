using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class CameraMovement : MonoBehaviour
{
    GameObject player;
    public Vector3 offset;
    bool inHouse = false;
    public bool on = false;
    public bool off = false;
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
        //
        if(on == true)
        {
            player.GetComponent<Movement>().move = false;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 1, Time.deltaTime);

            if (Camera.main.fieldOfView <= 28)
            {
                on = false;
                Camera.main.fieldOfView = 28;
            }
        }
        if (off == true)
        {
            player.GetComponent<Movement>().move = false;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 51, Time.deltaTime * 4);

            if (Camera.main.fieldOfView >= 50)
            {
                off = false;
                Camera.main.fieldOfView = 50;
            }
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