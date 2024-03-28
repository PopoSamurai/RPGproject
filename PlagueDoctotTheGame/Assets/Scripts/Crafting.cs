using UnityEngine;

public class Crafting : MonoBehaviour
{
    GameObject player;
    public bool interactOn = false;
    public GameObject CraftPanelWin;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (player.GetComponent<Movement>().interactOn == true && interactOn == true)
        {
            player.GetComponent<Movement>().interactClick = true;
            CraftPanelWin.SetActive(true);
            interactOn = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            interactOn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Movement>().interactClick = false;
            interactOn = false;
            CraftPanelWin.SetActive(false);
        }
    }
    public void CloseWin()
    {
        player.GetComponent<Movement>().interactClick = false;
        CraftPanelWin.SetActive(false);
    }
}
