using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float moveUpSpeed = 40f;
    public float lifeTime = 1f;
    Text _text;
    Color _startColor;
    void Awake()
    {
        _text = GetComponent<Text>();
        _startColor = _text.color;
    }
    public void Init(int value, bool isHeal)
    {
        _text.text = value.ToString();
        _text.color = isHeal ? Color.green : Color.red;
        _startColor = _text.color;

        StartCoroutine(AnimRoutine());
    }
    IEnumerator AnimRoutine()
    {
        float t = 0f;
        RectTransform rt = (RectTransform)transform;
        Vector3 startPos = rt.anchoredPosition;

        while (t < lifeTime)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = startPos + Vector3.up * moveUpSpeed * t;
            float alpha = Mathf.Lerp(1f, 0f, t / lifeTime);
            _text.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);

            yield return null;
        }
        Destroy(gameObject);
    }
}