using UnityEngine;
public class GameManager : MonoBehaviour
{
    public bool isSelected = false;
    [SerializeField] private Transform prefabParent;
    [SerializeField] private GameObject characterSelectedMark;
    GameObject player;
    public Collider2D hit;
    //
    public GameObject battlePanel;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && player.GetComponent<Movement>().interact == false)
        {
            Vector2 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.OverlapPoint(mouse_position);

            if (hit)
            {
                Debug.Log("select");
                isSelected = true;
                player.GetComponent<Movement>().MovePlayer();
                player.GetComponent<Movement>().clickMark.SetActive(false);
                characterSelectedMark.transform.SetParent(prefabParent);
                characterSelectedMark.SetActive(true);
                characterSelectedMark.transform.position = hit.transform.position;
                //Invoke("DeSelect", 1f);
            }
        }
    }
    public void DeSelect()
    {
        isSelected = false;
        characterSelectedMark.SetActive(false);
    }
}