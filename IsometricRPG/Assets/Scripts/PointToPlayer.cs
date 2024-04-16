using UnityEngine;
public class PointToPlayer : MonoBehaviour
{
    public RectTransform canvasRectT;
    public RectTransform aim;
    public Transform player;

    void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, player.position);

        aim.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
    }
}