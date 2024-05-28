using UnityEngine;
public class GameManager : MonoBehaviour
{
    public bool isSelected = false;
    [SerializeField] private Transform prefabParent;
    [SerializeField] private GameObject characterSelectedMark;
    GameObject player;
    public Collider2D hit;
    Sprite frame;
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

            if (hit && hit.GetComponent<Interact>().active == false)
            {
                frame = hit.transform.Find("frame").GetComponent<SpriteRenderer>().sprite;
                battlePanel.GetComponent<BattleSystem>().enemy1 = hit.GetComponent<Interact>().enemy;
                battlePanel.GetComponent<BattleSystem>().player1 = player.GetComponent<Movement>().main;
                isSelected = true;
                player.GetComponent<Movement>().MovePlayer();
                player.GetComponent<Movement>().clickMark.SetActive(false);
                characterSelectedMark.transform.SetParent(prefabParent);
                characterSelectedMark.SetActive(true);
                characterSelectedMark.transform.position = hit.transform.position;
                characterSelectedMark.GetComponent<SpriteRenderer>().sprite = frame;
            }
        }
    }
    public void DeSelect()
    {
        isSelected = false;
        characterSelectedMark.SetActive(false);
    }
}