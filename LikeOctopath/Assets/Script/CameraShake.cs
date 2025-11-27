using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    Vector3 _originalPos;
    Coroutine _shakeRoutine;

    void Awake()
    {
        Instance = this;
        _originalPos = transform.localPosition;
    }
    public void Shake(float duration, float magnitude)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }
    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 basePos = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            Vector2 offset2D = Random.insideUnitCircle * magnitude;
            Vector3 offset = new Vector3(offset2D.x, offset2D.y, 0f);

            transform.position = basePos + offset;

            yield return null;
        }
        transform.position = basePos;
        _shakeRoutine = null;
    }
}