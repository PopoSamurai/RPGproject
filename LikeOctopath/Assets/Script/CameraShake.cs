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
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector3 offset = Random.insideUnitSphere * magnitude;
            transform.localPosition = _originalPos + offset;

            yield return null;
        }
        transform.localPosition = _originalPos;
        _shakeRoutine = null;
    }
}