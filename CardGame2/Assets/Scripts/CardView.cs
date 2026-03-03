using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CardView : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public CardData data;
    [HideInInspector]
    public Unit AttachedUnit;

    [Header("UI")]
    public Image artworkImage;
    public Text nameText;
    public Text costText;

    [Header("Groups")]
    public GameObject HPStat;
    public GameObject AttackStat;
    public Text hpText;
    public Text attackText;
    public Text effectText;

    private Transform originalParent;
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;

    private int originalSiblingIndex;
    private Vector2 originalAnchoredPos;
    public bool IsDragging { get; set; }
    public bool WasPlayedOnBoard = false;

    [HideInInspector]
    public BoardSlot CurrentSlot;
    public SlotOwner owner;
    void Awake()
    {
        AttachedUnit = GetComponent<Unit>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = FindObjectOfType<Canvas>();
    }
    public void ReturnToSlot()
    {
        if (CurrentSlot == null) return;

        RectTransform rt = GetComponent<RectTransform>();
        rt.SetParent(CurrentSlot.transform, false);
        rt.localPosition = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        WasPlayedOnBoard = true;
    }
    public void ReturnToHand()
    {
        WasPlayedOnBoard = false;

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        transform.localRotation = Quaternion.identity;

        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = originalAnchoredPos;

        var hover = GetComponent<CardHoverUI>();
        if (hover) hover.enabled = true;

        FindObjectOfType<ArcLayoutGroup>().UpdateLayout();
        FindObjectOfType<ArcLayoutGroup>().UpdateLayout(instant: true);
    }
    private void Update()
    {
        if (IsDragging)
        {
            transform.SetAsLastSibling();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK: " + data.cardName);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data.type == CardType.Attack
         || data.type == CardType.Heal
         || data.type == CardType.BuffAttack)
        {
            CardTargetLine.Instance.StartLine(GetComponent<RectTransform>());
            return;
        }
        IsDragging = true;

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        var rt = GetComponent<RectTransform>();
        originalAnchoredPos = rt.anchoredPosition;

        transform.SetParent(mainCanvas.transform, true);
        transform.SetAsLastSibling();

        transform.rotation = Quaternion.identity;
        canvasGroup.blocksRaycasts = false;

        var hover = GetComponent<CardHoverUI>();
        if (hover) hover.enabled = false;
    }
    public void UpdateStatsUI()
    {
        if (AttachedUnit != null)
        {
            if (attackText != null)
                attackText.text = AttachedUnit.attack.ToString();
            if (hpText != null)
                hpText.text = AttachedUnit.currentHP.ToString();
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (data.type == CardType.Attack || data.type == CardType.Heal || data.type == CardType.BuffAttack)
        {
            CardTargetLine.Instance.StopLine();

            var targetGO = TargetDetector.Instance.CurrentTarget;
            if (targetGO == null)
            {
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, hits);
                foreach (var hit in hits)
                {
                    var slot = hit.gameObject.GetComponent<BoardSlot>();
                    if (slot != null && slot.transform.childCount > 0)
                    {
                        var boardTarget = slot.transform.GetChild(0).GetComponent<BoardTarget>();
                        if (boardTarget != null)
                        {
                            targetGO = boardTarget;
                            break;
                        }
                    }
                }
            }
            if (targetGO != null)
            {
                var boardTarget = targetGO.GetComponent<BoardTarget>();
                var unit = boardTarget.GetComponent<Unit>();
                var cardView = targetGO.GetComponent<CardView>();
                if (unit != null)
                {
                    switch (data.type)
                    {
                        case CardType.Attack:
                            unit.TakeDamage(data.damageAmount);
                            cardView?.AttachedUnit?.TakeDamage(data.attack);
                            cardView?.UpdateStatsUI();
                            break;
                        case CardType.Heal:
                            unit.Heal(data.healAmount);
                            cardView?.AttachedUnit?.Heal(data.hp);
                            cardView?.UpdateStatsUI();
                            break;
                        case CardType.BuffAttack:
                            cardView?.AttachedUnit?.AddAttack(data.buffAttackAmount);
                            cardView?.UpdateStatsUI();
                            break;
                    }
                }

                FindObjectOfType<HandManager>().RemoveCard(this);
                FindObjectOfType<ArcLayoutGroup>().UpdateLayout(true);
                return;
            }
            Debug.Log("[TARGET] No valid target");
            return;
        }
        if (data.type == CardType.Energy)
        {
            if (IsOutsideHand())
            {
                var energySystem = FindObjectOfType<PlayerEnergySystem>();
                energySystem.AddEnergy(data.energyAmount);
                FindObjectOfType<HandManager>().RemoveCard(this);
                FindObjectOfType<ArcLayoutGroup>().UpdateLayout(true);
                Destroy(gameObject);
                return;
            }
            else
            {
                ReturnToHand();
                return;
            }
        }
        BoardSlot targetSlot = null;
        List<RaycastResult> rayHits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, rayHits);

        foreach (var hit in rayHits)
        {
            var slot = hit.gameObject.GetComponent<BoardSlot>();
            if (slot != null && slot.owner != SlotOwner.Player)
            {
                Debug.Log("WRACAJ");
                targetSlot = slot;
                break;
            }
        }
        if (targetSlot != null)
        {
            if (!targetSlot.occupied && CurrentSlot == null)
            {
                CardExecutor.Instance.TryPlayUnitCard(this, targetSlot, true, false);
            }
            else if (CurrentSlot != null)
            {
                if (targetSlot.owner != SlotOwner.Player)
                {
                    ReturnToSlot();
                    return;
                }
                var targetCard = targetSlot.transform.childCount > 0 ?
                    targetSlot.transform.GetChild(0).GetComponent<CardView>() : null;

                if (targetCard != null)
                {
                    if (targetCard.owner != SlotOwner.Player)
                    {
                        ReturnToSlot();
                        return;
                    }
                    var oldSlot = CurrentSlot;

                    RectTransform rt1 = GetComponent<RectTransform>();
                    RectTransform rt2 = targetCard.GetComponent<RectTransform>();

                    rt1.SetParent(targetSlot.transform, false);
                    rt1.localPosition = Vector3.zero;
                    rt1.localRotation = Quaternion.identity;
                    rt1.localScale = Vector3.one;

                    rt2.SetParent(oldSlot.transform, false);
                    rt2.localPosition = Vector3.zero;
                    rt2.localRotation = Quaternion.identity;
                    rt2.localScale = Vector3.one;

                    targetCard.CurrentSlot = oldSlot;
                    CurrentSlot = targetSlot;

                    oldSlot.occupied = true;
                    targetSlot.occupied = true;
                }
                else
                {
                    CardExecutor.Instance.TryPlayUnitCard(this, targetSlot, false, false);
                }
            }
            else
            {
                ReturnToHand();
            }
        }
        else
        {
            if (CurrentSlot != null)
            {
                ReturnToSlot();
            }
            else
            {
                ReturnToHand();
            }
        }
    }
    bool IsOutsideHand()
    {
        var hand = FindObjectOfType<HandManager>().transform as RectTransform;
        var cardRT = GetComponent<RectTransform>();

        return !RectTransformUtility.RectangleContainsScreenPoint(
            hand,
            Input.mousePosition,
            mainCanvas.worldCamera
        );
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (data.type == CardType.Attack
            || data.type == CardType.Heal
            || data.type == CardType.BuffAttack)
            return;

        transform.position = Input.mousePosition;
    }
    public void Init(CardData cardData)
    {
        data = cardData;

        nameText.text = data.cardName;
        artworkImage.sprite = data.artwork;
        costText.text = data.energyCost.ToString();

        if (data.type != CardType.Character)
        {
            HPStat.gameObject.SetActive(false);
            AttackStat.gameObject.SetActive(false);
        }
        switch (data.type)
        {
            case CardType.Character:
                hpText.gameObject.SetActive(true);
                attackText.gameObject.SetActive(true);
                hpText.text = data.hp.ToString();
                attackText.text = data.attack.ToString();
                break;

            case CardType.Attack:
                effectText.gameObject.SetActive(true);
                effectText.text = $"Deal {data.damageAmount} dmg";
                break;

            case CardType.Heal:
                effectText.gameObject.SetActive(true);
                effectText.text = $"Heal {data.healAmount}";
                break;

            case CardType.Energy:
                effectText.gameObject.SetActive(true);
                effectText.text = $"+{data.energyAmount} Energy";
                break;

            case CardType.BuffAttack:
                effectText.gameObject.SetActive(true);
                effectText.text = $"Buff {data.buffAttackAmount} ATK";
                break;
        }
    }
}