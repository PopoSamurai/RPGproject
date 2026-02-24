using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRangedChaser2D : MonoBehaviour
{
    [Header("Cele")]
    public Transform player;

    [Header("Ruch")]
    public float moveSpeed = 3f;
    public float shootRange = 8f;       // zasiêg, w którym mo¿e strzelaæ
    public float stopDistance = 3f;     // minimalny dystans – bli¿ej nie podchodzi, jeœli widzi gracza

    [Header("Strzelanie")]
    public GameObject bulletPrefab;     // Twój prefab z Bullet2D
    public Transform firePoint;         // Punkt wylotu pocisku
    public float fireRate = 1f;         // strza³y / sekunda

    [Header("Kolizje / przeszkody")]
    public LayerMask obstacleMask;      // warstwa œcian / przeszkód (BEZ gracza)

    private Rigidbody2D rb;
    private float fireTimer;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        FindPlayerAtStart();
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 myPos = rb.position;
        Vector2 targetPos = player.position;
        Vector2 dir = (targetPos - myPos).normalized;
        float dist = Vector2.Distance(myPos, targetPos);

        // SprawdŸ, czy coœ zas³ania liniê miêdzy wrogiem a graczem
        bool blocked = Physics2D.Raycast(myPos, dir, dist, obstacleMask);

        // Czy wróg jest gotowy do strza³u?
        bool canShoot = dist <= shootRange && !blocked;

        // --- RUCH ---
        if (!canShoot || dist > stopDistance)
        {
            // Podchodzimy do gracza (gonimy go)
            rb.linearVelocity = dir * moveSpeed;
        }
        else
        {
            anim.SetBool("attack", false);
            // Jesteœmy w dobrym miejscu do strza³u -> stoimy
            rb.linearVelocity = Vector2.zero;
        }

        // Opcjonalnie obróæ przeciwnika twarz¹ do gracza
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // --- STRZELANIE ---
        fireTimer -= Time.deltaTime;
        if (canShoot && fireTimer <= 0f)
        {
            anim.SetBool("attack", true);
            Shoot(dir);
            fireTimer = 1f / fireRate;
        }
    }

    private void Shoot(Vector2 dir)
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Obracamy pocisk tak, ¿eby jego transform.right lecia³ w stronê gracza
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject b = Instantiate(bulletPrefab, firePoint.position, rot);

        // Je¿eli w prefabie pocisku u¿ywasz jednak kierunku "up"
        // (useTransformRight = false), mo¿esz to obs³u¿yæ tak:
        /*
        Bullet2D bullet = b.GetComponent<Bullet2D>();
        if (bullet != null && !bullet.useTransformRight)
        {
            // Ustawiamy tak, ¿eby transform.up by³ w stronê gracza
            b.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }
        */
    }

    // Dla podgl¹du w edytorze
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
    void FindPlayerAtStart()
    {
        // 1) jeœli ju¿ rêcznie przypisany w Inspectorze – nic nie rób
        if (player != null) return;

        // 2) spróbuj znaleŸæ obiekt z tagiem "Player"
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            return;
        }

        // 3) awaryjnie: znajdŸ pierwszy obiekt z komponentem PlayerController (albo innym skryptem gracza)
        // Podmieñ PlayerController na nazwê swojego skryptu od gracza
        var playerController = FindAnyObjectByType<PlayerAimAnimToMouse>();
        if (playerController != null)
        {
            player = playerController.transform;
            return;
        }

        // 4) jeœli nic nie znalaz³ – daj info w konsoli
        Debug.LogWarning($"[EnemyRangedChaser2D] Nie znaleziono gracza w scenie! Ustaw tag 'Player' albo podepnij rêcznie w Inspectorze. ({name})");
    }
}