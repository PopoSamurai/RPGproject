using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    private Rigidbody rb;
    private Vector3 moveDir;
    private Vector2 animDir;
    private Vector2 lastMoveDir = Vector2.down;
    SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        ReadInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Move();
    }
    void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // --- ANIMACJA: czysty input WSAD ---
        animDir = new Vector2(h, v).normalized;
        if (animDir.magnitude > 0.01f)
            lastMoveDir = animDir;

        // --- RUCH: kamera-relative ---
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        moveDir = (forward * v + right * h).normalized;
    }
    void Move()
    {
        Vector3 velocity = moveDir * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }
    void UpdateAnimation()
    {
        if (animDir.x < 0)
            sr.flipX = false;
        else if (animDir.x > 0)
            sr.flipX = true;

        bool isMoving = animDir.magnitude > 0.01f;
        animator.SetBool("move", isMoving);
        if (isMoving)
        {
            animator.SetFloat("moveX", animDir.x);
            animator.SetFloat("moveY", animDir.y);
        }
        else
        {
            animator.SetFloat("moveX", lastMoveDir.x);
            animator.SetFloat("moveY", lastMoveDir.y);
        }
    }
}