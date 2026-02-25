using System.Collections.Generic;
using UnityEngine;
public class CardTargetLine : MonoBehaviour
{
    public static CardTargetLine Instance;

    public RectTransform canvasRect;
    public GameObject segmentPrefab;
    public int segmentCount = 25;
    public float curveStrength = 120f;
    private List<RectTransform> segments = new List<RectTransform>();
    private RectTransform startPoint;
    private bool active;
    public bool snapToTarget = true;
    void Awake()
    {
        Instance = this;
    }
    public void StartLine(RectTransform from)
    {
        ClearLine();

        startPoint = from;
        active = true;

        for (int i = 0; i < segmentCount; i++)
        {
            var seg = Instantiate(segmentPrefab, canvasRect).GetComponent<RectTransform>();
            segments.Add(seg);
        }
    }
    public void StopLine()
    {
        active = false;
        ClearLine();
    }
    void Update()
    {
        if (!active || startPoint == null) return;

        Vector2 start = startPoint.position + Vector3.up * 40f;
        Vector2 end = Input.mousePosition;
        BoardTarget target = TargetDetector.Instance?.Detect();
        if (snapToTarget && target != null && target.snapPoint != null)
        {
            end = target.snapPoint.position;
        }

        float dist = Vector2.Distance(start, end);
        float curvePower = Mathf.Clamp(dist * 0.3f, 120f, 400f);
        Vector2 control = (start + end) / 2f + Vector2.up * curvePower;

        for (int i = 0; i < segments.Count; i++)
        {
            float t = i / (float)(segments.Count - 1);
            Vector2 pos = Bezier(start, control, end, t);
            segments[i].position = pos;

            float scale = Mathf.Lerp(1f, 0.3f, t);
            segments[i].localScale = Vector3.one * scale;
        }
    }
    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return (1 - t) * (1 - t) * a +
               2 * (1 - t) * t * b +
               t * t * c;
    }
    void ClearLine()
    {
        foreach (var s in segments)
            if (s) Destroy(s.gameObject);

        segments.Clear();
    }
}