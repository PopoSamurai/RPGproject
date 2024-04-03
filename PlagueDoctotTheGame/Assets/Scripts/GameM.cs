using UnityEngine;
using UnityEngine.UI;

public class GameM : MonoBehaviour
{
    public GameObject CraftPanelWin;
    public GameObject InventoryPanel;
    public GameObject MenuOptions;
    public GameObject DialogPanel;
    GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (CraftPanelWin.activeSelf == true || InventoryPanel.activeSelf == true || MenuOptions.activeSelf == true || DialogPanel.activeSelf == true)
        {
            player.GetComponent<Movement>().MoveOff();
        }
    }
    public void offWindows()
    {
        CraftPanelWin.SetActive(false);
        InventoryPanel.SetActive(false);
        MenuOptions.SetActive(false);
        DialogPanel.SetActive(false);
    }
}