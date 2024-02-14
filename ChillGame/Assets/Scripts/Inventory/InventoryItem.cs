using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Text countText;

    [HideInInspector] public Item item;
    public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Transform firstPos;
    public GameObject statWin;
    GameObject gamem;
    public int costItem = 0;
    private void Start()
    {
        gamem = GameObject.FindGameObjectWithTag("gamem");
        statWin.SetActive(false);
    }
    public void InitializeItem(Item newItem)
    {
        firstPos = transform.parent;
        item = newItem;
        image.sprite = newItem.icon;
        RefreshCount();
    }
    public void SepareteItems(Item newItem)
    {
        count -= 1;
        firstPos = transform.parent;
        item = newItem;
        image.sprite = newItem.icon;
        RefreshCount();
    }
    public void RefreshCount()
    {
        costItem = item.costTosell * count;
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (count > 1 && Input.GetKey(KeyCode.R))
        {
            SepareteItems(item);
            firstPos = transform.parent;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            GameObject obj = Instantiate(this.gameObject, firstPos);
            obj.GetComponent<InventoryItem>().count = count;
            count = 1;
            RefreshCount();
            image.raycastTarget = false;
        }
        else
        {
            firstPos = transform.parent;
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f;
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        statWin.SetActive(true);
        if(gamem.GetComponent<GameManager>().panels[2].activeSelf == true)
        {
            statWin.transform.GetChild(0).GetComponent<Text>().text = item.nameItem;
        }
        else
        {
            statWin.transform.GetChild(0).GetComponent<Text>().text = costItem + "g"; 
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        statWin.SetActive(false);
    }
}