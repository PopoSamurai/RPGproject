using UnityEngine;
using UnityEngine.EventSystems;
public class CardHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{ 
    public float hoverOffset = 60f;
    public float animSpeed = 10f;
    private RectTransform rt; 
    private Vector2 basePos; 
    private bool isHovered; 
    private int baseSiblingIndex; 
    private static CardHoverUI currentHovered;
    void Awake() 
    { 
        rt = GetComponent<RectTransform>(); 
        basePos = rt.anchoredPosition; 
        baseSiblingIndex = transform.GetSiblingIndex(); 
    } 
    public void SetBasePosition(Vector2 pos) 
    { 
        basePos = pos; 
    } 
    public void OnPointerEnter(PointerEventData eventData) 
    { 
        if (currentHovered != null && currentHovered != this) 
        { 
            currentHovered.Hide(); 
        } 
        currentHovered = this; 
        isHovered = true; 
        baseSiblingIndex = transform.GetSiblingIndex(); 
        transform.SetAsLastSibling(); 
    } 
    public void OnPointerExit(PointerEventData eventData) 
    { 
        if (currentHovered == this) 
        { 
            Hide(); 
            currentHovered = null; 
        } 
    } 
    public void Hide() 
    { 
        isHovered = false; 
        transform.SetSiblingIndex(baseSiblingIndex); 
    } 
    void Update()
    { 
        var cv = GetComponent<CardView>(); 
        if (cv != null && cv.IsDragging) 
            return; Vector2 target = basePos;

        if (isHovered) target = basePos + Vector2.up * hoverOffset; 
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, target, Time.deltaTime * animSpeed); 
    }
}