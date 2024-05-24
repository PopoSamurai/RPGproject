using UnityEngine;
using UnityEngine.UI;
public class NPC : MonoBehaviour
{
    public GameObject interact;
    int i;
    public float speed;
    public int startpoint;
    public Transform[] points;
    Rigidbody rb;
    public int ID { get; set; }
    //
    public static bool dialogOn = false;
    public DialogReader reader;
    public DialogPref dialog, endDialog;
    public bool finalDialog = false;
    public GameObject dialogBox;
    bool move = true;
    public Quest[] quest;
    public bool addQuest;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        finalDialog = false;
        transform.position = points[startpoint].position;
        ID = 0;
    }
    void Update()
    {
        if (dialogOn && Input.GetKeyDown(KeyCode.E))
        {
            interact.SetActive(false);
            finalDialog = true;
            reader.GetComponent<DialogReader>().UpdateUI();
            dialogBox.SetActive(true);
            Movement.move = false;
            dialogOn = false;
        }
        if (move == true)
        {
            if (Vector3.Distance(transform.position, points[i].position) < 0.02f)
            {
                i++;
                if (i == points.Length)
                {
                    i = 0;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
        else
        {
           rb.velocity = Vector3.zero;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interact.SetActive(true);
            if (reader.dialogText.text != null && !finalDialog)
            {
                move = false;
                reader.StartDialog(dialog);
            }
            else
            {
                move = false;
                reader.StartDialog(endDialog);
            }
            dialogOn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        move = true;
        interact.SetActive(false);
        dialogOn = false;
    }
}