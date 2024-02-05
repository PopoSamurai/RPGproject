using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject dialogBox;
    public bool dialogOn = false;
    public Material baseMat, outLine;
    SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(dialogOn && Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.SetActive(true);
            Movement.move = false;
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
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
