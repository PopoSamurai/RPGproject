using System.Collections;
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
    public GameObject CounterStat;
    public GameObject color;
    public Sprite CounterYellow, CounterRed;
    public Text hpText;
    public Text attackText;
    public Text counterText;
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

    private int dragStartIndex = -1;
    private BoardLane dragStartLane;
    public bool IsLocked => owner == SlotOwner.Enemy;
    void Awake()
    {
        color.gameObject.GetComponent<Image>().sprite = CounterYellow;
        if (owner == SlotOwner.Enemy && canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = FindObjectOfType<Canvas>();

        if (data && data.type == CardType.Character)
        {
            UpdateStatsUI();
            CounterStat.SetActive(true);
        }
        else CounterStat.SetActive(false);
    }
    public void BindUnit(Unit unit)
    {
        AttachedUnit = unit;
        UpdateStatsUI();
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
        if (!TurnManager.Instance.IsPlayerTurn)
            return;

        if (owner != SlotOwner.Player)
        {
            IsDragging = false;

            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = true;
            return;
        }

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

        dragStartLane = CurrentSlot?.line;
        dragStartIndex = dragStartLane != null
            ? dragStartLane.GetCardIndex(this)
            : -1;

        var hover = GetComponent<CardHoverUI>();
        if (hover) hover.enabled = false;
    }
    public void UpdateStatsUI()
    {
        if (AttachedUnit != null)
        {
            attackText.text = AttachedUnit.attack.ToString();
            hpText.text = AttachedUnit.currentHP.ToString();
            counterText.text = AttachedUnit.counter.ToString();

            if (AttachedUnit.counter == 1)
                color.GetComponent<Image>().sprite = CounterRed;
            else
                color.GetComponent<Image>().sprite = CounterYellow;
        }
        else if (data != null)
        {
            attackText.text = data.attack.ToString();
            hpText.text = data.hp.ToString();
            counterText.text = data.couner.ToString();

            color.GetComponent<Image>().sprite = CounterYellow;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!TurnManager.Instance.IsPlayerTurn)
        {
            ReturnToSlot();
            return;
        }

        if (owner != SlotOwner.Player)
        {
            ReturnToSlot();
            return;
        }
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
        BoardLane targetLane = null;
        List<RaycastResult> rayHits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, rayHits);

        foreach (var hit in rayHits)
        {
            var lane = hit.gameObject.GetComponentInParent<BoardLane>();
            if (lane != null && lane.owner == SlotOwner.Player)
            {
                targetLane = lane;
                break;
            }
        }
        if (targetLane == null)
        {
            if (CurrentSlot != null)
                ReturnToSlot();
            else
                ReturnToHand();
            return;
        }

        int insertIndex = targetLane.GetInsertIndex(transform.position);
        BoardSlot targetSlot = targetLane.slots[insertIndex];
        float dist = Mathf.Abs(transform.position.x - targetSlot.transform.position.x);
        bool isOnSlotCenter = dist < 30f;
        bool shouldInsertBetween = !isOnSlotCenter;
        BoardLane oldLane = CurrentSlot?.line;

        var targetCard = targetSlot.transform.childCount > 0
            ? targetSlot.transform.GetChild(0).GetComponent<CardView>()
            : null;

        if (CurrentSlot == null)
        {
            if (targetLane.IsFull())
            {
                ReturnToHand();
                return;
            }

            bool success = CardExecutor.Instance.TryPlayUnitCard(
                this,
                targetLane.slots[0],
                true,
                false
            );

            if (!success)
            {
                ReturnToHand();
                return;
            }

            targetLane.InsertCard(this, insertIndex);
            return;
        }
        if (targetCard != null && !shouldInsertBetween)
        {
            if (targetCard.owner == SlotOwner.Enemy)
            {
                ReturnToSlot();
                return;
            }
            SwapCards(this, targetCard);
            return;
        }
        if (targetLane.IsFull())
        {
            ReturnToSlot();
            return;
        }
        if (CurrentSlot != null)
            CurrentSlot.occupied = false;

        oldLane?.CompactLine();
        targetLane.InsertCard(this, insertIndex);
    }
    void SwapCards(CardView a, CardView b)
    {
        if (a.owner == SlotOwner.Enemy || b.owner == SlotOwner.Enemy)
        {
            a.ReturnToSlot();
            return;
        }
        if (a.IsLocked || b.IsLocked)
        {
            a.ReturnToSlot();
            return;
        }
        var slotA = a.CurrentSlot;
        var slotB = b.CurrentSlot;

        var unitA = a.AttachedUnit;
        var unitB = b.AttachedUnit;

        if (unitA != null) unitA.CurrentSlot = slotB;
        if (unitB != null) unitB.CurrentSlot = slotA;

        if (slotA == null || slotB == null) return;

        var rtA = a.GetComponent<RectTransform>();
        var rtB = b.GetComponent<RectTransform>();

        rtA.SetParent(slotB.transform, false);
        rtB.SetParent(slotA.transform, false);

        rtA.localPosition = Vector3.zero;
        rtB.localPosition = Vector3.zero;
        rtA.localScale = Vector3.one;
        rtB.localScale = Vector3.one;

        a.CurrentSlot = slotB;
        b.CurrentSlot = slotA;

        slotA.occupied = true;
        slotB.occupied = true;
    }
    public IEnumerator FlashHP()
    {
        if (hpText == null)
            yield break;

        Color original = hpText.color;
        hpText.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        hpText.color = original;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!TurnManager.Instance.IsPlayerTurn)
            return;

        if (owner != SlotOwner.Player)
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

        if (data.type != CardType.Character)
        {
            HPStat.gameObject.SetActive(false);
            AttackStat.gameObject.SetActive(false);
            CounterStat.SetActive(false);
        }
        else CounterStat.SetActive(true);
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

            case CardType.BuffAttack:
                effectText.gameObject.SetActive(true);
                effectText.text = $"Buff {data.buffAttackAmount} ATK";
                break;
        }
        UpdateStatsUI();
    }
}