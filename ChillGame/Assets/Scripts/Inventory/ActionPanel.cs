using UnityEngine;
using UnityEngine.UI;
public class ActionPanel : MonoBehaviour
{
    public Image spriteIco;
    public InventorySlot slot;
    Button buttonActive;
    private void Start()
    {
        buttonActive = GetComponent<Button>();
        spriteIco.sprite = null;
    }
    void Update()
    {
        if(slot.transform.childCount == 0)
        {
            spriteIco.sprite = null;
            buttonActive.enabled = false;
        }
        else
        {
            spriteIco.sprite = slot.transform.GetChild(0).GetComponent<InventoryItem>().image.sprite;
            buttonActive.enabled = true;
        }
    }
}
