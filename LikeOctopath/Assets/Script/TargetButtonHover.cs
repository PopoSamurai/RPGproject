using UnityEngine;
using UnityEngine.EventSystems;

public class TargetButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager.Instance?.PreviewHighlight(index, true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.Instance?.PreviewHighlight(index, false);
    }
}