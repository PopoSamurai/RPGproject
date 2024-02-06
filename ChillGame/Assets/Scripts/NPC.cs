using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject dialogBox;
    public bool dialogOn = false;
    public Material baseMat, outLine;
    SpriteRenderer sr;
    public DialogReader reader;
    public Dialog dialog, endDialog;
    public bool finalDialog = false;

    void Start()
    {
        finalDialog = false;
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (dialogOn && Input.GetKeyDown(KeyCode.E))
        {
            reader.GetComponent<DialogReader>().UpdateUI();
            dialogBox.SetActive(true);
            Movement.move = false;
            dialogOn = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (reader.dialogText.text != null && !finalDialog)
            {
                reader.StartDialog(dialog);
                finalDialog = true;
            }
            else
            {
                reader.StartDialog(endDialog);
            }
            sr.material = outLine;
            dialogOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        sr.material = baseMat;
        dialogOn = false;
    }
}
