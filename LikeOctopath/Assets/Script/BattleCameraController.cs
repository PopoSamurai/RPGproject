using System.Collections;
using UnityEngine;
public class BattleCameraController : MonoBehaviour
{
    public static BattleCameraController Instance;

    [Header("BREAK zoom")]
    [SerializeField] float breakTimeScale = 0.2f;
    [SerializeField] float zoomInTime = 0.15f;
    [SerializeField] float holdTime = 0.15f;
    [SerializeField] float zoomOutTime = 0.2f;
    [SerializeField] float targetYOffset = 0.5f;

    [Header("Ortho camera")]
    [SerializeField] float zoomOrthoSize = 3.5f;

    private Camera _cam;
    private Vector3 _defaultPos;
    private float _defaultOrthoSize;
    private float _defaultFov;
    private bool _isOrtho;
    private bool _isPlaying;

    [Header("Optional")]
    public MonoBehaviour otherCameraController;
    private Vector3 _breakCamPos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _cam = GetComponent<Camera>();
        if (_cam == null) return;
        _defaultPos = _cam.transform.position;
        _breakCamPos = _defaultPos;
        _isOrtho = _cam.orthographic;

        if (_isOrtho)
            _defaultOrthoSize = _cam.orthographicSize;
        else
            _defaultFov = _cam.fieldOfView;
    }
    void LateUpdate()
    {
        if (_cam == null) return;
        if (_isPlaying)
        {
            _cam.transform.position = _breakCamPos;
        }
    }
    public void PlayBreakZoom(Vector3 focusWorldPos, string debugName = "")
    {
        if (_cam == null) return;
        if (_isPlaying) return;

        StartCoroutine(BreakZoomRoutine(focusWorldPos, debugName));
    }
    private IEnumerator BreakZoomRoutine(Vector3 focusWorldPos, string debugName)
    {
        _isPlaying = true;

        if (otherCameraController != null)
            otherCameraController.enabled = false;

        float startTimeScale = Time.timeScale;
        Vector3 startPos = _cam.transform.position;
        float startOrthoSize = _isOrtho ? _cam.orthographicSize : 0f;
        float startFov = !_isOrtho ? _cam.fieldOfView : 0f;
        Vector3 focusPos = focusWorldPos + new Vector3(0f, targetYOffset, 0f);
        Vector3 zoomPos = new Vector3(focusPos.x, focusPos.y, startPos.z);
        Time.timeScale = breakTimeScale;

        float t = 0f;
        while (t < zoomInTime)
        {
            t += Time.unscaledDeltaTime;
            float lerp = Mathf.Clamp01(t / zoomInTime);

            _breakCamPos = Vector3.Lerp(startPos, zoomPos, lerp);

            if (_isOrtho)
                _cam.orthographicSize = Mathf.Lerp(startOrthoSize, zoomOrthoSize, lerp);
            else
                _cam.fieldOfView = Mathf.Lerp(startFov, _defaultFov * 0.7f, lerp);

            yield return null;
        }
        float hold = 0f;
        while (hold < holdTime)
        {
            hold += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = startTimeScale;

        float tBack = 0f;
        Vector3 fromPos = _breakCamPos;
        float fromOrtho = _isOrtho ? _cam.orthographicSize : 0f;
        float fromFov2 = !_isOrtho ? _cam.fieldOfView : 0f;

        while (tBack < zoomOutTime)
        {
            tBack += Time.unscaledDeltaTime;
            float lerp = Mathf.Clamp01(tBack / zoomOutTime);

            _breakCamPos = Vector3.Lerp(fromPos, _defaultPos, lerp);

            if (_isOrtho)
                _cam.orthographicSize = Mathf.Lerp(fromOrtho, _defaultOrthoSize, lerp);
            else
                _cam.fieldOfView = Mathf.Lerp(fromFov2, _defaultFov, lerp);

            yield return null;
        }
        _breakCamPos = _defaultPos;
        _cam.transform.position = _defaultPos;

        if (_isOrtho) _cam.orthographicSize = _defaultOrthoSize;
        else _cam.fieldOfView = _defaultFov;

        if (otherCameraController != null)
            otherCameraController.enabled = true;

        _isPlaying = false;
    }
}