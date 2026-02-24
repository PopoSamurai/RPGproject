using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAimAnimToMouse : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private string paramX = "x";
    [SerializeField] private string paramY = "y";
    [SerializeField] private string paramMove = "move";

    public float moveDeadzone = 0.05f;

    public Rigidbody2D rb;
    private Camera cam;
    private Vector2 lastLookDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector2 input = Vector2.zero;
        if (kb.aKey.isPressed) input.x -= 1;
        if (kb.dKey.isPressed) input.x += 1;
        if (kb.wKey.isPressed) input.y += 1;
        if (kb.sKey.isPressed) input.y -= 1;

        Vector2 moveDir = input.sqrMagnitude > 0.0001f ? input.normalized : Vector2.zero;
        rb.linearVelocity = moveDir * moveSpeed;

        Vector2 lookDir = GetMouseDir();
        if (lookDir.sqrMagnitude > 0.0001f)
            lastLookDir = lookDir;

        bool isMoving = rb.linearVelocity.magnitude > moveDeadzone;

        if (animator)
        {
            animator.SetFloat(paramX, lastLookDir.x);
            animator.SetFloat(paramY, lastLookDir.y);
            animator.SetBool(paramMove, isMoving);
        }
    }
    private Vector2 GetMouseDir()
    {
        if (Mouse.current == null || cam == null) return Vector2.zero;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Vector3 mouseWorld;
        if (cam.orthographic)
        {
            mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        }
        else
        {
            float depth = Mathf.Abs(cam.transform.position.z - transform.position.z);
            if (depth < 0.01f) depth = 0.01f;
            mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, depth));
        }

        mouseWorld.z = transform.position.z;
        Vector2 dir = (Vector2)(mouseWorld - transform.position);
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.zero;
    }
}