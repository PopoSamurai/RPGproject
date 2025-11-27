using UnityEngine;
public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    public DamageText damageTextPrefab;
    public Canvas canvas;

    void Awake()
    {
        Instance = this;
    }
    public void ShowDamageText(int value, Vector3 worldPos, bool isHeal, bool fromPlayerSide, bool isSpell = false)
    {
        if (damageTextPrefab == null || canvas == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        DamageText dt = Instantiate(damageTextPrefab, canvas.transform);
        dt.transform.position = screenPos;

        Vector2 dir = fromPlayerSide ? Vector2.left : Vector2.right;
        dt.Init(value, isHeal, dir, worldPos, isSpell);
    }
    public void ShowCustomText(string text, Vector3 worldPos, Color color, bool fromPlayerSide)
    {
        if (damageTextPrefab == null || canvas == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        DamageText dt = Instantiate(damageTextPrefab, canvas.transform);
        dt.transform.position = screenPos;

        Vector2 dir = fromPlayerSide ? Vector2.left : Vector2.right;
        dt.InitText(text, color, dir, worldPos);
    }
}