using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using System.Collections.Generic;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Card hoveredCard;

    [HideInInspector] public UnityEvent<Card> beginDrag;
    [HideInInspector] public UnityEvent<Card> endDrag;
    [HideInInspector] public UnityEvent<Card> pointEnter;
    [HideInInspector] public UnityEvent<Card> pointExit;
    [HideInInspector] public UnityEvent<Card, bool> ppointUp;
    [HideInInspector] public UnityEvent<Card> pointDonw;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;
    public float selectionOffset = 50;
    public bool selected;
    private Vector3 originalPosition;
    private Transform originalParent;
    private RectTransform rectTransform;

    private float selectOffset = 50f;
    [HideInInspector] public bool isDrag;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("error 1");
            return;
        }

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
        {
            Debug.LogError("error 2");
        }

        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDrag)
        {
            selected = !selected;
            if (selected)
            {
                rectTransform.DOAnchorPos(originalPosition + new Vector3(0, selectionOffset, 0), 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                rectTransform.DOAnchorPos(originalPosition, 0.2f).SetEase(Ease.OutBack);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            Vector2 localPoint;

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPoint);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform, eventData.position, null, out localPoint);
            }

            rectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        hoveredCard = GetCardUnderPointer(eventData);

        if (hoveredCard != null)
        {
            SwapCards(hoveredCard);
        }
        else
        {
            transform.SetParent(originalParent);
            rectTransform.DOAnchorPos(originalPosition, 0.2f).SetEase(Ease.OutBack);
        }
    }

    private void SwapCards(Card otherCard)
    {
        Transform tempParent = otherCard.transform.parent;
        Vector3 tempPosition = otherCard.rectTransform.anchoredPosition;
        bool tempSelected = otherCard.selected;

        otherCard.transform.SetParent(originalParent);
        transform.SetParent(tempParent);

        otherCard.rectTransform.anchoredPosition = originalPosition;
        rectTransform.anchoredPosition = tempPosition;

        otherCard.selected = selected;
        selected = tempSelected;
    }

    private Card GetCardUnderPointer(PointerEventData eventData)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            Card card = result.gameObject.GetComponent<Card>();
            if (card != null && card != this)
            {
                return card;
            }
        }

        return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDrag)
        {
            hoveredCard = eventData.pointerEnter?.GetComponent<Card>();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoveredCard == eventData.pointerEnter?.GetComponent<Card>())
        {
            hoveredCard = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerDown(PointerEventData eventData) { }

    internal int ParentIndex()
    {
        throw new NotImplementedException();
    }
}