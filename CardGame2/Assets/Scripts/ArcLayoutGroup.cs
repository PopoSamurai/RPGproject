using UnityEngine;
using System.Collections.Generic;
public class ArcLayoutGroup : MonoBehaviour
{
    public float radius;
    public float angleStep;
    public bool rotateToArc = true;
    private HandManager handManager;
    void Awake()
    {
        handManager = GetComponent<HandManager>();
    }
    public void UpdateLayout(bool instant = false)
    {
        if (handManager == null) return;

        List<CardView> cards = handManager.hand;
        int count = cards.Count;
        if (count == 0) return;

        float centerIndex = (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            var card = cards[i];
            if (card == null) continue;
            if (card.IsDragging) continue;

            var rt = card.GetComponent<RectTransform>();
            float offset = i - centerIndex;
            float angle = offset * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * radius;
            float y = Mathf.Cos(rad) * radius - radius;
            Vector2 targetPos = new Vector2(x, y);

            if (instant)
            {
                rt.anchoredPosition = targetPos;
                if (rotateToArc)
                    rt.localRotation = Quaternion.Euler(0, 0, -angle);
            }
            else
            {
                rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, Time.deltaTime * 12f);
                if (rotateToArc)
                    rt.localRotation = Quaternion.Lerp(rt.localRotation, Quaternion.Euler(0, 0, -angle), Time.deltaTime * 12f);
            }

            var hover = card.GetComponent<CardHoverUI>();
            if (hover != null)
                hover.SetBasePosition(targetPos);

            card.transform.SetSiblingIndex(i);
        }
    }
    void OnValidate()
    {
        UpdateLayout();
    }
    void Start()
    {
        UpdateLayout();
    }
}