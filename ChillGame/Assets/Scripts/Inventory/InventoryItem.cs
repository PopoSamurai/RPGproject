using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public Text countText;

    [HideInInspector] public Item item;
    public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Transform firstPos;
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
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (count >= 1 && Input.GetKey(KeyCode.R))
        {
            Debug.Log("separate");
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
}