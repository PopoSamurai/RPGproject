using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Image image;

    public event Action<Item> OnRightClickEvent;

    public Item _item;
    public int itemCount;
    public Item Item

    {   get { return _item; }
        set { 
            _item = value;
            if (_item == null)
            {
                image.enabled = false;
            }
            else
            {
                image.sprite = _item.itemIcon;
                image.enabled = true;
            }
        }
    }
    protected virtual void OnValidate()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null && OnRightClickEvent != null)
                OnRightClickEvent(Item);
        }
    }
}
