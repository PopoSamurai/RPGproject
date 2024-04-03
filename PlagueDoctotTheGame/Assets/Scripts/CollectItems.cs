using UnityEngine;
using UnityEngine.UI;

public enum CollectObject
{
    none,
    bush,
    rock,
    tree,
    table
}
public class CollectItems : MonoBehaviour
{
    public CollectObject type;
    GameObject player;
    public bool interactOn = false;
    public bool startNum = false;
    float counDown;
    int number;
    [Header("change material")]
    public Material defaultMat;
    public Material changeMat;
    public bool collected = false;
    public BoxCollider coll;
    GameObject gameM;
    private void Start()
    {
        counDown = 4f;
        player = GameObject.FindGameObjectWithTag("Player");
        gameM = GameObject.FindGameObjectWithTag("GameManager");
    }
    private void Update()
    {
        switch(type)
        {
            case CollectObject.none:
                if (counDown <= 1.0f)
                {
                    coll.enabled = false;
                    startNum = false;
                    endItem();
                    Debug.Log("add item");
                }
                break;
            case CollectObject.bush:
                if (counDown <= 1.0f)
                {
                    coll.enabled = false;
                    startNum = false;
                    endItem();
                    Debug.Log("add berries");
                }
                break;
            case CollectObject.rock:
                if (counDown <= 1.0f)
                {
                    coll.enabled = false;
                    startNum = false;
                    endItem();
                    Debug.Log("add stone");
                }
                break;
            case CollectObject.tree:
                if (counDown <= 1.0f)
                {
                    startNum = false;
                    endTree();
                    Debug.Log("add wood");
                }
                break;
            case CollectObject.table:
                startNum = false;
                break;
        }
        //odliczanie
        if(startNum == true)
        {
            counDown -= Time.deltaTime;
            number = Mathf.FloorToInt(counDown);
            player.GetComponent<Movement>().collectInteract.SetActive(true);
            player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount -= 1.0f / counDown * Time.deltaTime;
            player.GetComponent<Movement>().collectTxt.GetComponent<Text>().text = number.ToString();
        }

        if (player.GetComponent<Movement>().interactOn == true && interactOn == true)
        {
            if (type == CollectObject.table)
                gameM.GetComponent<GameM>().CraftPanelWin.SetActive(true);

            startNum = true;
            player.GetComponent<Movement>().interactClick = true;
            interactOn = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && collected == false)
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
        }
    }
    public void endItem()
    {
        interactOn = false;
        GetComponent<MeshRenderer>().material = changeMat;
        player.GetComponent<Movement>().interactClick = false;
        counDown = 4f;
        player.GetComponent<Movement>().collectInteract.SetActive(false);
        player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount = 1;
        collected = true;
    }
    public void endTree()
    {
        interactOn = false;
        player.GetComponent<Movement>().interactClick = false;
        counDown = 4f;
        player.GetComponent<Movement>().collectInteract.SetActive(false);
        player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount = 1;
        collected = true;
        Destroy(this.gameObject);
    }
    public void endTable()
    {
        interactOn = false;
        player.GetComponent<Movement>().interactClick = false;
        player.GetComponent<Movement>().move = true;
        gameM.GetComponent<GameM>().CraftPanelWin.SetActive(false);
    }
}