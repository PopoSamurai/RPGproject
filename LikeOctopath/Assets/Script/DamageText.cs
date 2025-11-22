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

    private Text _text;
    private Color _startColor;
    private Vector2 _direction = Vector2.up;
    void Awake()
    {
        if (_text == null)
            _text = GetComponent<Text>();
    }
    public void Init(int value, bool isHeal, Vector2 direction)
    {
        if (_text == null)
            _text = GetComponent<Text>();

        if (value < 0)
        {
            _text.text = "BREAK";
            _text.color = Color.cyan;
        }
        else
        {
            _text.text = value.ToString();
            _text.color = isHeal ? Color.green : Color.white;
        }

        _startColor = _text.color;
        _direction = direction.normalized;

        StartCoroutine(AnimRoutine());
    }
    public void InitText(string text, Color color, Vector2 direction)
    {
        if (_text == null)
            _text = GetComponent<Text>();

        _text.text = text;
        _text.color = color;

        _startColor = color;
        _direction = direction.normalized;

        StartCoroutine(AnimRoutine());
    }
    IEnumerator AnimRoutine()
    {
        if (_text == null)
            _text = GetComponent<Text>();

        RectTransform rt = _text.rectTransform;
        Vector2 startPos = rt.anchoredPosition;
        float time = 0f;

        while (time < lifeTime)
        {
            time += Time.deltaTime;
            float lerp = Mathf.Clamp01(time / lifeTime);

            float curve = moveCurve.Evaluate(lerp);
            Vector2 moveOffset = _direction * moveDistance * curve;

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