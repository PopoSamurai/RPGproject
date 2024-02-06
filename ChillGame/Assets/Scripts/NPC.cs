using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject dialogBox;
    public bool dialogOn = false;
    public Material baseMat, outLine;
    SpriteRenderer sr;
    GameObject reader;
    public Dialog dialog, endDialog;
    public bool dialogOff;
    void Start()
    {
        dialogOff = true;
        sr = GetComponent<SpriteRenderer>();
        reader = GameObject.FindGameObjectWithTag("UI");
    }

    void Update()
    {
        if(dialogOn && Input.GetKeyDown(KeyCode.E))
        {
            reader.GetComponent<DialogReader>().UpdateUI();
            dialogBox.SetActive(true);
            Movement.move = false;
            dialogOff = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(dialogOff)
            {
                reader.GetComponent<DialogReader>().currentDialog = dialog;
                reader.GetComponent<DialogReader>().end = endDialog;
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
