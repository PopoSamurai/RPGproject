using UnityEngine;
using UnityEngine.UI;
public enum CollectObject
{
    none,
    bush,
    rock,
    tree
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
    private void Start()
    {
        counDown = 4f;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        switch(type)
        {
            case CollectObject.none:
                if (counDown <= 1.0f)
                {
                    startNum = false;
                    endNone();
                    Debug.Log("add item");
                }
                break;
            case CollectObject.bush:
                if (counDown <= 1.0f)
                {
                    startNum = false;
                    endBerry();
                    Debug.Log("add berries");
                }
                break;
            case CollectObject.rock:
                if (counDown <= 1.0f)
                {
                    startNum = false;
                    endBerry();
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
            player.GetComponent<Movement>().interactClick = true;
            player.GetComponent<Movement>().interactOn = false;
            player.GetComponent<Movement>().move = false;
            interactOn = true;
            startNum = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && collected == false)
        {
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
    public void endBerry()
    {
        interactOn = false;
        GetComponent<MeshRenderer>().material = changeMat;
        player.GetComponent<Movement>().interactClick = false;
        player.GetComponent<Movement>().interactOn = false;
        player.GetComponent<Movement>().move = true;
        counDown = 4f;
        player.GetComponent<Movement>().interact.SetActive(false);
        player.GetComponent<Movement>().collectInteract.SetActive(false);
        player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount = 1;
        collected = true;
    }
    public void endTree()
    {
        interactOn = false;
        player.GetComponent<Movement>().interactClick = false;
        player.GetComponent<Movement>().interactOn = false;
        player.GetComponent<Movement>().move = true;
        counDown = 4f;
        player.GetComponent<Movement>().interact.SetActive(false);
        player.GetComponent<Movement>().collectInteract.SetActive(false);
        player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount = 1;
        collected = true;
        Destroy(this.gameObject);
    }
    public void endNone()
    {
        interactOn = false;
        GetComponent<MeshRenderer>().material = changeMat;
        player.GetComponent<Movement>().interactClick = false;
        player.GetComponent<Movement>().interactOn = false;
        player.GetComponent<Movement>().move = true;
        counDown = 4f;
        player.GetComponent<Movement>().interact.SetActive(false);
        player.GetComponent<Movement>().collectInteract.SetActive(false);
        player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount = 1;
        collected = true;
    }
    public void endStone()
    {
        interactOn = false;
        GetComponent<MeshRenderer>().material = changeMat;
        player.GetComponent<Movement>().interactClick = false;
        player.GetComponent<Movement>().interactOn = false;
        player.GetComponent<Movement>().move = true;
        counDown = 4f;
        player.GetComponent<Movement>().interact.SetActive(false);
        player.GetComponent<Movement>().collectInteract.SetActive(false);
        player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount = 1;
        collected = true;
    }
}