using UnityEngine;

// The character's logical facing is represented by FrontPoint (a child of the unit,
// NOT of the billboard). Only the sprite is rotated towards the camera.
[DefaultExecutionOrder(10000)]
[RequireComponent(typeof(TacticalUnit))]
public class TacticalUnitSprite : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Camera gameCamera;
    [Tooltip("Child of Unit, outside Visual. Its blue Z arrow indicates the character's front.")]
    public Transform frontPoint;

    [Header("Appearance")]
    public bool hideCapsuleMesh = true;
    public Vector3 visualLocalOffset = Vector3.zero;
    public Vector3 initialFacing = Vector3.forward;

    [Header("Animation")]
    [Min(0.1f)] public float idleFramesPerSecond = 3f;
    [Min(0.1f)] public float walkFramesPerSecond = 8f;

    [Header("Isometric camera view")]
    [Tooltip("Correction of the camera's diagonal isometric angle. Usually 45 or -45 degrees. Changes ONLY which sprite is selected, not the visual rotation.")]
    public float viewAngleOffset = 45f;

    [Header("Debug")]
    public bool showFacingGizmo = true;

    private TacticalUnit unit;
    private Transform visual;
    private Vector3 facing;
    private bool walking;
    private bool walkingExplicit;
    private float animationTime;

    private void Awake()
    {
        unit = GetComponent<TacticalUnit>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        if (spriteRenderer != null) visual = spriteRenderer.transform;

        facing = Flat(initialFacing);
        if (facing.sqrMagnitude < 0.0001f) facing = Vector3.forward;
        facing.Normalize();

        // Initial direction is explicitly configured on the unit, not inferred
        // from a rotated marker or a billboard.
        FaceDirection(facing);
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

    private static Vector3 Flat(Vector3 value)
    {
        value.y = 0f;
        return value;
    }

    private void ApplyFacingToMarker()
    {
        if (frontPoint != null && facing.sqrMagnitude > 0.0001f)
            frontPoint.rotation = Quaternion.LookRotation(facing, Vector3.up);
    }

    // Called by movement, attacks, throws, etc. Uses WORLD-space direction.
    public void FaceDirection(Vector3 worldDirection)
    {
        Vector3 flat = Flat(worldDirection);
        if (flat.sqrMagnitude < 0.0001f) return;
        // Four cardinal grid directions only. Never store a diagonal facing.
        facing = Mathf.Abs(flat.x) >= Mathf.Abs(flat.z)
            ? (flat.x >= 0f ? Vector3.right : Vector3.left)
            : (flat.z >= 0f ? Vector3.forward : Vector3.back);
        ApplyFacingToMarker();
        RefreshSprite();
    }

    public void FaceTarget(Vector3 targetWorldPosition)
    {
        FaceDirection(targetWorldPosition - transform.position);
    }

    public void SetWalking(bool value)
    {
        walkingExplicit = value;
        if (walking != value)
        {
            walking = value;
            animationTime = 0f;
        }
    }

    private void LateUpdate()
    {
        if (gameCamera == null) gameCamera = Camera.main;
        if (walking != walkingExplicit)
        {
            walking = walkingExplicit;
            animationTime = 0f;
        }
        else animationTime += Time.deltaTime;
        RefreshSprite();
    }

    private void RefreshSprite()
    {
        if (spriteRenderer == null || visual == null || unit == null ||
            unit.unitData == null || gameCamera == null) return;

        // Billboard follows the actual camera; animation direction follows the
        // TARGET yaw. The camera's smooth interpolation must never switch frames.
        Vector3 cameraFacing = Flat(-gameCamera.transform.forward);
        if (cameraFacing.sqrMagnitude < 0.0001f) return;
        cameraFacing.Normalize();

        TacticalCameraController orbit = gameCamera.GetComponent<TacticalCameraController>();
        float cameraYaw = orbit != null ? orbit.TargetYaw : gameCamera.transform.eulerAngles.y;
        Vector3 viewDirection = Quaternion.Euler(0f, cameraYaw + viewAngleOffset, 0f) * Vector3.back;
        // Snap the viewing direction to the closest WORLD cardinal axis.
        // Isometric cameras at 45 degrees must not sit on a front/side boundary.
        viewDirection = Mathf.Abs(viewDirection.x) >= Mathf.Abs(viewDirection.z)
            ? (viewDirection.x >= 0f ? Vector3.right : Vector3.left)
            : (viewDirection.z >= 0f ? Vector3.forward : Vector3.back);

        // Facing is a logical grid direction, set by movement/attack/jump.
        Vector3 front = facing;
        Vector3 right = Vector3.Cross(Vector3.up, front);
        float frontDot = Vector3.Dot(viewDirection, front);
        float rightDot = Vector3.Dot(viewDirection, right);
        int direction = Mathf.Abs(frontDot) >= Mathf.Abs(rightDot)
            ? (frontDot >= 0f ? 0 : 1)
            : (rightDot >= 0f ? 2 : 3);

        TacticalUnitData data = unit.unitData;
        Sprite fallback = direction == 0 ? data.frontSprite :
                          direction == 1 ? data.backSprite :
                          direction == 2 ? data.rightSprite : data.leftSprite;
        Sprite[] frames = walking
            ? (direction == 0 ? data.walkFront : direction == 1 ? data.walkBack :
               direction == 2 ? data.walkRight : data.walkLeft)
            : (direction == 0 ? data.idleFront : direction == 1 ? data.idleBack :
               direction == 2 ? data.idleRight : data.idleLeft);

        Sprite selected = null;
        if (frames != null && frames.Length > 0)
        {
            int count = 0;
            for (int i = 0; i < frames.Length; i++)
                if (frames[i] != null) count++;
            if (count > 0)
            {
                float fps = walking ? walkFramesPerSecond : idleFramesPerSecond;
                int index = Mathf.FloorToInt(animationTime * fps) % count;
                for (int i = 0; i < frames.Length; i++)
                {
                    if (frames[i] == null) continue;
                    if (index-- == 0) { selected = frames[i]; break; }
                }
            }
        }
        if (selected == null) selected = fallback;
        if (selected != null) spriteRenderer.sprite = selected;

        visual.rotation = Quaternion.LookRotation(cameraFacing, Vector3.up);
        visual.localPosition = visualLocalOffset;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showFacingGizmo) return;
        Vector3 direction = frontPoint != null ? Flat(frontPoint.forward) :
            (Application.isPlaying ? facing : Flat(initialFacing));
        if (direction.sqrMagnitude < 0.0001f) direction = Vector3.forward;
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.up * 0.15f;
        Gizmos.DrawRay(origin, direction.normalized * 0.9f);
        Gizmos.DrawSphere(origin + direction.normalized * 0.9f, 0.08f);
    }
}
