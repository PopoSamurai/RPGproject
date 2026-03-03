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
        if (owner == SlotOwner.Enemy)
            return;

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
            if (slot != null && slot.owner == SlotOwner.Player)
            {
                Debug.Log("WRACAJ");
                targetSlot = slot;
                break;
            }
        }
        if (targetSlot != null && targetSlot.owner != SlotOwner.Player)
        {
            ReturnToHand();
            return;
        }
        if (targetSlot == null)
        {
            if (CurrentSlot != null)
                ReturnToSlot();
            else
                ReturnToHand();
            return;
        }
        // blokada swapa z wrogiem
        if (targetSlot.owner != SlotOwner.Player)
        {
            if (CurrentSlot != null)
                ReturnToSlot();
            else
                ReturnToHand();
            return;
        }
        if (CurrentSlot == null)
        {
            if (!targetSlot.occupied)
            {
                bool success = CardExecutor.Instance.TryPlayUnitCard(this, targetSlot, true, false);

                if (!success)
                    ReturnToHand();
            }
            else
            {
                ReturnToHand();
            }
            return;
        }
        if (targetSlot == CurrentSlot)
        {
            ReturnToSlot();
            return;
        }
        if (!targetSlot.occupied)
        {
            MoveToSlot(targetSlot);
            return;
        }
        CardView otherCard =
            targetSlot.transform.GetChild(0).GetComponent<CardView>();

        if (otherCard != null && otherCard.owner == SlotOwner.Player)
        {
            SwapWith(otherCard);
        }
        else
        {
            ReturnToSlot();
        }
    }
    void MoveToSlot(BoardSlot newSlot)
    {
        CurrentSlot.occupied = false;

        RectTransform rt = GetComponent<RectTransform>();
        rt.SetParent(newSlot.transform, true);

        rt.localScale = Vector3.one;
        rt.localPosition = Vector3.zero;

        newSlot.occupied = true;
        CurrentSlot = newSlot;
    }
    void SwapWith(CardView other)
    {
        if (other.owner != SlotOwner.Player)
            return;

        if (other.CurrentSlot.owner != SlotOwner.Player)
            return;

        BoardSlot mySlot = CurrentSlot;
        BoardSlot otherSlot = other.CurrentSlot;

        RectTransform myRT = GetComponent<RectTransform>();
        RectTransform otherRT = other.GetComponent<RectTransform>();

        myRT.SetParent(otherSlot.transform, true);
        otherRT.SetParent(mySlot.transform, true);

        myRT.localScale = Vector3.one;
        otherRT.localScale = Vector3.one;

        myRT.localPosition = Vector3.zero;
        otherRT.localPosition = Vector3.zero;

        CurrentSlot = otherSlot;
        other.CurrentSlot = mySlot;

        mySlot.occupied = true;
        otherSlot.occupied = true;
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
        if (owner == SlotOwner.Enemy)
            return;

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