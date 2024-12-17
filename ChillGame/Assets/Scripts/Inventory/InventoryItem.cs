using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Text countText;

    [HideInInspector] public Item item;
    public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Transform firstPos;
    GameObject subGameObject;
    public int costItem = 0;
    Vector3 temp = new Vector3(1.5f, 1.5f, 0);
    public bool isSplitting = false; // Flaga podzia³u przedmiotów
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
            isSplitting = true;
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
        if (isSplitting == true)
        {
            if (parentAfterDrag.childCount == 0)
            {
                if (firstPos.childCount > 0)
                {
                    firstPos.GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                    firstPos.GetChild(0).GetComponent<InventoryItem>().isSplitting = false;
                    Debug.Log("1");
                }
            }
            else
            {
                if (firstPos.childCount > 0)
                {
                    firstPos.GetChild(0).GetComponent<InventoryItem>().count += count;
                    firstPos.GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                    firstPos.GetChild(0).GetComponent<InventoryItem>().isSplitting = false;
                }
                Destroy(gameObject);
                Debug.Log("2");
            }
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
            isSplitting = false;
        }
        else
        {
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
            isSplitting = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        subGameObject = Instantiate(Resources.Load("ItemStats", typeof(GameObject))) as GameObject;
        subGameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        subGameObject.transform.position = eventData.pointerEnter.transform.position + temp;
        subGameObject.transform.GetChild(0).GetComponent<Text>().text = item.nameItem + '\n' + "<color=green>" + item.type.ToString() + "</color>" + '\n' + '\n' + "<color=yellow>" + costItem + "g" + "</color>";
        InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(subGameObject);
    }
}