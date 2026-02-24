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
    private Vector3 originalPosition;
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;

    private int originalSiblingIndex;
    private Vector2 originalAnchoredPos;
    public bool IsDragging { get; set; }
    public bool WasPlayedOnBoard = false;
    void Awake()
    {
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
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        IsDragging = false;

        if (!WasPlayedOnBoard)
        {
            ReturnToHand();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
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
        }
    }
}