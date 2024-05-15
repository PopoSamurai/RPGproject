using UnityEngine;
public class TeleportScr : MonoBehaviour
{
    GameObject player;
    public Transform point1;
    public Transform point2;
    public GameObject skipWin;
    public bool tp = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (player.GetComponent<Movement>().interactOn == true && tp == true)
        {
            skipWin.SetActive(true);
            Movement.move = false;
            Invoke("waitForMove", 1f);
            player.transform.position = point2.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("go house");
            player.GetComponent<Movement>().interact.SetActive(true);
            tp = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Movement>().interact.SetActive(false);
        }
    }
    void waitForMove()
    {
        Movement.move = true;
        skipWin.SetActive(false);
        tp = false;
    }
}