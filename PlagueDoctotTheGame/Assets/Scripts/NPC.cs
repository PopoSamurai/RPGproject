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
                player.GetComponent<Movement>().dialogOn = true;
                dialogManager.currentDialog = endDialog;
                dialogManager.StartDialog(endDialog);
                interactOn = false;
            }
            else
            {
                player.GetComponent<Movement>().dialogOn = true;
                dialogManager.currentDialog = currentDialog;
                dialogManager.StartDialog(currentDialog);
                interactOn = false;
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
            interactOn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Movement>().dialogOn = false;
            player.GetComponent<Movement>().interactClick = false;
            interactOn = false;
        }
    }
    public void CloseWin()
    {
        player.GetComponent<Movement>().dialogOn = false;
        dialogoff = true;
        player.GetComponent<Movement>().interactClick = false;
    }
}