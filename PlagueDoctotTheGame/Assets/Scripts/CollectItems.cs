using UnityEngine;
using UnityEngine.UI;
public class CollectItems : MonoBehaviour
{
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
        //odliczanie
        if(startNum == true)
        {
            counDown -= Time.deltaTime;
            number = Mathf.FloorToInt(counDown);
            player.GetComponent<Movement>().collectInteract.SetActive(true);
            player.GetComponent<Movement>().LoadBar.GetComponent<Image>().fillAmount -= 1.0f / counDown * Time.deltaTime;
            player.GetComponent<Movement>().collectTxt.GetComponent<Text>().text = number.ToString();
            if (counDown <= 1.0f)
            {
                GetComponent<MeshRenderer>().material = changeMat;
                startNum = false;
                endTimer();
                Debug.Log("add berries");
            }
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
    public void endTimer()
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
    }
}