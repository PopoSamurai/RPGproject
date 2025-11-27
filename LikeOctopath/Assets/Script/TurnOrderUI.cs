using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurnOrderUI : MonoBehaviour
{
    [Tooltip("Sloty obrazków na górze (w kolejnoœci z lewej do prawej).")]
    public Image[] slots;

    [Header("Kolory")]
    public Color currentTurnColor = Color.white;
    public Color futureTurnColor = new Color(1f, 1f, 1f, 0.6f); 
    public Color enemyTint = new Color(1f, 0.85f, 0.85f, 1f);
    public Color playerTint = Color.white;

    public void Refresh(List<BattleUnit> order, BattleUnit currentUnit)
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < order.Count && order[i] != null && !order[i].IsDead)
            {
                var unit = order[i];
                var img = slots[i];
                if (img == null) continue;

                img.gameObject.SetActive(true);
                img.sprite = unit.data != null ? unit.data.sprite : null;
                img.preserveAspect = true;
                Color baseColor = unit.IsPlayer ? playerTint : enemyTint;

                if (unit == currentUnit)
                {
                    img.color = currentTurnColor * baseColor;
                }
                else
                {
                    img.color = futureTurnColor * baseColor;
                }
            }
            else
            {
                if (slots[i] != null)
                    slots[i].gameObject.SetActive(false);
            }
        }
    }
}