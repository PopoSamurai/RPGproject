using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Ruch")]
    public float moveDistance = 60f;
    public float lifeTime = 0.7f;

    [Header("Shake")]
    public float shakeMagnitude = 2f;
    public float shakeFrequency = 25f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Text _text;
    Color _startColor;
    Vector2 _direction;

    public void Awake()
    {
        _text = GetComponent<Text>();
        _startColor = _text.color;
    }
    public void Init(int value, bool isHeal, Vector2 direction)
    {
        _text.text = value.ToString();
        _text.color = isHeal ? Color.green : Color.white;
        _startColor = _text.color;
        _direction = direction.normalized;

        StartCoroutine(AnimRoutine());
    }
    IEnumerator AnimRoutine()
    {
        float t = 0f;
        RectTransform rt = (RectTransform)transform;
        Vector2 startPos = rt.anchoredPosition;

        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / lifeTime);

            Vector2 baseMoveDir = (_direction + Vector2.up * 0.2f).normalized;
            Vector2 moveOffset = baseMoveDir * moveDistance * moveCurve.Evaluate(lerp);

            float currentMag = shakeMagnitude * (1f - lerp);
            float angle = Time.time * shakeFrequency;
            Vector2 shakeOffset = new Vector2(
                Mathf.PerlinNoise(angle, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, angle) - 0.5f
            ).normalized * currentMag;
            rt.anchoredPosition = startPos + moveOffset + shakeOffset;

            float alpha = Mathf.Lerp(1f, 0f, lerp);
            _text.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}