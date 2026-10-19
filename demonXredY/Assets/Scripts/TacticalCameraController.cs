using UnityEngine;
[RequireComponent(typeof(Camera))]
public class TacticalCameraController : MonoBehaviour
{
    [Header("Cel")]
    public Transform target;
    public Vector3 targetOffset = Vector3.zero;
    public float targetHeight = 0.8f;
    public bool followTarget = true;
    [Min(0f)] public float followSmoothness = 8f;

    [Header("Izometryczny widok")]
    [Range(20f, 75f)] public float pitch = 38f;
    public float rotationStep = 90f;
    [Min(0f)] public float rotationSmoothness = 8f;

    [Header("Zoom")]
    [Tooltip("Orthographic: Camera Size. Perspective: odległość od celu.")]
    public float minZoom = 4f;
    public float maxZoom = 16f;
    public float zoomSensitivity = 1.5f;
    [Min(0f)] public float zoomSmoothness = 8f;

    [Header("Przesuwanie (opcjonalne)")]
    public bool enablePanning = false;
    [Min(0.01f)] public float panSensitivity = 0.02f;

    private Camera cam;
    private float yaw;
    private float desiredYaw;

    // Docelowy obrot po Q/E, niezalezny od animacji przejscia kamery.
    public float TargetYaw => initialized ? desiredYaw : transform.eulerAngles.y;
    private float zoom;
    private float desiredZoom;
    private Vector3 focusPoint;
    private bool initialized;
    private void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("TacticalCameraController wymaga komponentu Camera.");
            enabled = false;
            return;
        }

        focusPoint = target != null
            ? GetTargetPoint()
            : transform.position + transform.forward * 8f;

        yaw = transform.eulerAngles.y;
        desiredYaw = yaw;

        zoom = cam.orthographic
            ? Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom)
            : Mathf.Clamp(Vector3.Distance(transform.position, focusPoint), minZoom, maxZoom);

        desiredZoom = zoom;
        initialized = true;

        ApplyPose();
    }

    private Vector3 GetTargetPoint()
    {
        if (target == null)
            return focusPoint;

        return target.position + Vector3.up * targetHeight + targetOffset;
    }

    // Wywołuj tę metodę po zaznaczeniu jednostki
    // albo po zmianie aktywnej jednostki w systemie tur.
    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
            return;

        target = newTarget;

        if (!initialized)
            return;

        // Kamera płynnie przejdzie do nowej jednostki.
        // Nie teleportujemy od razu punktu skupienia.
    }

    public void ClearTarget()
    {
        target = null;
    }

    private void LateUpdate()
    {
        if (!initialized)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            desiredYaw -= rotationStep;

        if (Input.GetKeyDown(KeyCode.E))
            desiredYaw += rotationStep;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.001f)
        {
            desiredZoom = Mathf.Clamp(
                desiredZoom - scroll * zoomSensitivity,
                minZoom,
                maxZoom
            );
        }

        if (enablePanning && Input.GetMouseButton(2))
        {
            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");

            Quaternion yawRotation = Quaternion.Euler(0f, desiredYaw, 0f);

            focusPoint -= yawRotation * Vector3.right
                * dx * panSensitivity * zoom;

            focusPoint -= yawRotation * Vector3.forward
                * dy * panSensitivity * zoom;
        }
        else if (followTarget && target != null)
        {
            focusPoint = Vector3.Lerp(
                focusPoint,
                GetTargetPoint(),
                SmoothFactor(followSmoothness)
            );
        }

        yaw = Mathf.LerpAngle(
            yaw,
            desiredYaw,
            SmoothFactor(rotationSmoothness)
        );

        zoom = Mathf.Lerp(
            zoom,
            desiredZoom,
            SmoothFactor(zoomSmoothness)
        );

        ApplyPose();
    }

    private float SmoothFactor(float speed)
    {
        return speed <= 0f
            ? 1f
            : 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime);
    }

    private void ApplyPose()
    {
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);

        if (cam.orthographic)
        {
            cam.orthographicSize = zoom;

            float cameraDistance = Mathf.Max(maxZoom, 10f);

            transform.position = focusPoint
                - orbitRotation * Vector3.forward * cameraDistance;
        }
        else
        {
            transform.position = focusPoint
                - orbitRotation * Vector3.forward * zoom;
        }

        transform.rotation = orbitRotation;
    }
}