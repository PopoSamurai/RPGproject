using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
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
    void Awake()
    {
        AttachedUnit = GetComponent<Unit>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = FindObjectOfType<Canvas>();
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
        if (data.type == CardType.Attack
            || data.type == CardType.Heal
            || data.type == CardType.BuffAttack)
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
                            if (cardView != null && cardView.AttachedUnit != null)
                            {
                                cardView.AttachedUnit.TakeDamage(data.attack);
                                cardView.UpdateStatsUI();
                            }
                            Debug.Log($"[ATTACK] {data.cardName} dealt {data.damageAmount} dmg to {unit.name}");
                            break;

                        case CardType.Heal:
                            unit.Heal(data.healAmount);
                            if (cardView != null && cardView.AttachedUnit != null)
                            {
                                cardView.AttachedUnit.Heal(data.hp);
                                cardView.UpdateStatsUI();
                            }
                            Debug.Log($"[HEAL] {data.cardName} healed {data.healAmount} HP on {unit.name}");
                            break;

                        case CardType.BuffAttack:
                            Debug.Log($"[BUFF] {data.cardName} buffed {unit.name} ATK by {data.buffAttackAmount}");
                            if (cardView != null && cardView.AttachedUnit != null)
                            {
                                cardView.AttachedUnit.AddAttack(data.buffAttackAmount);
                                cardView.UpdateStatsUI();
                            }
                            break;
                    }
                }
                var handManager = FindObjectOfType<HandManager>();
                handManager.RemoveCard(this);
                FindObjectOfType<ArcLayoutGroup>().UpdateLayout(true);

                return;
            }
            Debug.Log("[TARGET] No valid target");
            return;
        }
        canvasGroup.blocksRaycasts = true;
        IsDragging = false;

        if (data.type == CardType.Energy)
        {
            if (IsOutsideHand())
            {
                Debug.Log($"[ENERGY] Gained {data.energyAmount} energy");

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
        if (!WasPlayedOnBoard)
            ReturnToHand();
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