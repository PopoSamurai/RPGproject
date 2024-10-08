using System.Collections;
using UnityEngine;
public enum AreaType
{
    enemy,
    village,
    house
}
public class Teleport : MonoBehaviour
{
    GameObject player;
    public Transform point1;
    public Transform point2;
    public GameObject skipWin;
    public GameObject EnemyContainer;
    public AreaType type;
    public bool tp = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (tp)
        {
            switch (type)
            {
                case AreaType.enemy:
                    EnemyContainer.SetActive(true);
                    player.GetComponent<Movement>().posPlayer = 1;
                    break;
                case AreaType.village:
                    point2.GetComponentInParent<Teleport>().EnemyContainer.SetActive(false);
                    player.GetComponent<Movement>().posPlayer = 2;
                    break;
                case AreaType.house:
                    EnemyContainer.SetActive(false);
                    player.GetComponent<Movement>().posPlayer = 3;
                    break;

            }
        }
        //
        if (player.GetComponent<Movement>().interactOn == true && tp == true)
        {
            skipWin.SetActive(true);
            player.GetComponent<Movement>().move = false;
            Invoke("waitForMove", 1f);
            player.transform.position = point2.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(type == AreaType.house)
            {
                Debug.Log("go house");
                player.GetComponent<Movement>().interact.SetActive(true);
                tp = true;
            }
            else
            {
                tp = true;
                skipWin.SetActive(true);
                player.GetComponent<Movement>().move = false;
                Invoke("waitForMove", 1f);
                player.transform.position = point2.position;
            }
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
        player.GetComponent<Movement>().move = true;
        skipWin.SetActive(false);
        tp = false;
    }
}