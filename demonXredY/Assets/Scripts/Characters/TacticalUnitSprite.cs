using UnityEngine;

// Static 4-direction sprites. Updates after the orbit camera (execution order 100).
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(TacticalUnit))]
public class TacticalUnitSprite : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Camera gameCamera;

    [Header("Appearance")]
    public bool hideCapsuleMesh = true;
    public Vector3 visualLocalOffset = Vector3.zero;
    public Vector3 initialFacing = Vector3.forward;
    [Min(0.001f)] public float movementThreshold = 0.015f;

    private TacticalUnit unit;
    private Transform visual;
    private Vector3 facing;
    private Vector3 previousPosition;

    private void Awake()
    {
        unit = GetComponent<TacticalUnit>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        if (spriteRenderer != null) visual = spriteRenderer.transform;
        facing = Vector3.ProjectOnPlane(initialFacing, Vector3.up);
        if (facing.sqrMagnitude < 0.0001f) facing = Vector3.forward;
        facing.Normalize();
        previousPosition = transform.position;
    }

    private void Start()
    {
        if (gameCamera == null) gameCamera = Camera.main;
        if (hideCapsuleMesh)
        {
            MeshRenderer capsule = GetComponent<MeshRenderer>();
            if (capsule != null) capsule.enabled = false;
        }
    }

    public void FaceDirection(Vector3 worldDirection)
    {
        worldDirection.y = 0f;
        if (worldDirection.sqrMagnitude > 0.0001f) facing = worldDirection.normalized;
    }

    public void FaceTarget(Vector3 targetWorldPosition)
    {
        FaceDirection(targetWorldPosition - transform.position);
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null || visual == null || unit == null || unit.unitData == null) return;
        if (gameCamera == null) gameCamera = Camera.main;
        if (gameCamera == null) return;

        Vector3 delta = transform.position - previousPosition;
        previousPosition = transform.position;
        delta.y = 0f;
        if (delta.magnitude > movementThreshold * Mathf.Max(Time.deltaTime, 0.0001f))
            FaceDirection(delta);

        // Use camera orientation rather than its position. This stays stable
        // while the camera follows another unit or moves between targets.
        Vector3 toCamera = -gameCamera.transform.forward;
        toCamera.y = 0f;
        if (toCamera.sqrMagnitude < 0.0001f) return;
        toCamera.Normalize();

        // SignedAngle: positive = camera is on character's RIGHT in Unity XZ.
        float angle = Vector3.SignedAngle(facing, toCamera, Vector3.up);
        Sprite selected;
        if (Mathf.Abs(angle) <= 45f)
            selected = unit.unitData.frontSprite;
        else if (Mathf.Abs(angle) >= 135f)
            selected = unit.unitData.backSprite;
        else if (angle < 0f)
            selected = unit.unitData.rightSprite;
        else
            selected = unit.unitData.leftSprite;

        if (selected == null)
            selected = Vector3.Dot(facing, toCamera) >= 0f
                ? unit.unitData.frontSprite : unit.unitData.backSprite;
        if (selected != null) spriteRenderer.sprite = selected;

        // Upright vertical billboard; no X or Z tilt.
        visual.rotation = Quaternion.LookRotation(toCamera, Vector3.up);
        visual.localPosition = visualLocalOffset;
    }
}
