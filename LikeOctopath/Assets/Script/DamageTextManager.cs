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
    public void ShowDamageText(int value, Vector3 worldPos, bool isHeal)
    {
        if (damageTextPrefab == null || canvas == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        DamageText dt = Instantiate(damageTextPrefab, canvas.transform);
        dt.transform.position = screenPos;
        dt.Init(value, isHeal);
    }
}