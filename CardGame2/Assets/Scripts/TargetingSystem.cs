using UnityEngine;
public class TargetingSystem : MonoBehaviour
{
    public static TargetingSystem Instance;
    private CardData pendingCard;
    void Awake()
    {
        Instance = this;
    }
    public void StartTargeting(CardData card)
    {
        pendingCard = card;
        Debug.Log("Select target for " + card.cardName);
    }
    public void SelectTarget(GameObject target)
    {
        CardExecutor.Instance.UseCard(pendingCard, target, false);
        pendingCard = null;
    }
}