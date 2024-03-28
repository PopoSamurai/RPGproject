using UnityEngine;

public class NPC : MonoBehaviour
{
    GameObject player;
    public bool interactOn = false;
    public DialogManager dialogManager;
    public DialogReader currentDialog;
    public DialogReader endDialog;
    public bool dialogoff = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (player.GetComponent<Movement>().interactOn == true && interactOn == true)
        {
            if (dialogoff == true)
            {
                dialogManager.currentDialog = endDialog;
                player.GetComponent<Movement>().move = false;
                dialogManager.StartDialog(endDialog);
                player.GetComponent<Movement>().interactOn = false;
                interactOn = false;
                player.GetComponent<Movement>().interact.SetActive(false);
            }
            else
            {
                dialogManager.currentDialog = currentDialog;
                player.GetComponent<Movement>().move = false;
                dialogManager.StartDialog(currentDialog);
                player.GetComponent<Movement>().interactOn = false;
                interactOn = false;
                player.GetComponent<Movement>().interact.SetActive(false);
            }
        }
        //
        if (dialogManager.endDialog == true)
        {
            CloseWin();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogManager.endDialog = false;
            player.GetComponent<Movement>().interact.SetActive(true);
            interactOn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Movement>().interactOn = false;
            player.GetComponent<Movement>().interactClick = false;
            player.GetComponent<Movement>().interact.SetActive(false);
            interactOn = false;
            player.GetComponent<Movement>().move = true;
        }
    }
    public void CloseWin()
    {
        dialogoff = true;
        player.GetComponent<Movement>().interactOn = false;
        player.GetComponent<Movement>().interactClick = false;
        player.GetComponent<Movement>().move = true;
    }
}