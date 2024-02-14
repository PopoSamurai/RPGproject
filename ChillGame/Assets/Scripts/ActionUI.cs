using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
    }
}
