using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyExploder2D : MonoBehaviour
{
    [Header("NavMesh / Ruch")]
    public NavMeshAgent agent;
    public float repathInterval = 0.1f;

    [Header("Prefaby / Efekty")]
    public GameObject spawnOnDespawnPrefab; // co zostaje po wrogu
    public GameObject explodeVfxPrefab;     // opcjonalny VFX wybuchu

    [Header("Gracz / Tag / Auto-szukaj")]
    [Tooltip("Tag gracza, którego mamy œcigaæ i raniæ.")]
    public string playerTag = "Player";
    [Tooltip("Czy przeciwnik ma sam znajdowaæ gracza po tagu.")]
    public bool autoFindPlayerByTag = true;
    [Tooltip("Co ile sekund próbowaæ znaleŸæ/odœwie¿yæ cel.")]
    public float findInterval = 0.5f;
    [Tooltip("Jeœli znajdzie wielu, wybierze najbli¿szego.")]
    public bool pickNearestIfMany = true;

    // Referencje ustawiane automatycznie:
    Transform _player;

    // Referencje do stref (dzieci)
    [HideInInspector] public EnemyTriggerZone2D explodeZone; // Trigger 1
    [HideInInspector] public EnemyTriggerZone2D damageZone;  // Trigger 2

    Coroutine _repathCo, _findCo;
    bool _isExploded;

    //anim
    public Animator animator;
    public string paramX = "x";
    public string paramY = "y";

    public float stopSpeed = 0.03f;
    public float dirSmooth = 10f;
    public bool pauseAnimatorWhenStopped = true;
    private Vector2 lastDir = Vector2.down;
    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    void Update()
    {
        Vector2 dir = new Vector2(agent.velocity.x, agent.velocity.y);

        if (dir.sqrMagnitude < 0.0001f && agent.hasPath)
        {
            Vector3 t = agent.steeringTarget;
            dir = (Vector2)(t - transform.position);
        }

        if (dir.sqrMagnitude > 0.0001f)
        {
            Vector2 desired = dir.normalized;
            lastDir = Vector2.Lerp(lastDir, desired, 1f - Mathf.Exp(-dirSmooth * Time.deltaTime));
        }

        animator.SetFloat(paramX, lastDir.x);
        animator.SetFloat(paramY, lastDir.y);

        if (pauseAnimatorWhenStopped)
        {
            float speed = agent.velocity.magnitude;
            animator.speed = (speed > stopSpeed || agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                             ? 1f : 0f;
        }
    }

    void OnEnable()
    {
        if (_repathCo == null) _repathCo = StartCoroutine(RepathLoop());
        if (autoFindPlayerByTag && _findCo == null) _findCo = StartCoroutine(FindPlayerLoop());
    }

    void OnDisable()
    {
        if (_repathCo != null) { StopCoroutine(_repathCo); _repathCo = null; }
        if (_findCo != null) { StopCoroutine(_findCo); _findCo = null; }
        if (agent) agent.ResetPath();
    }

    IEnumerator RepathLoop()
    {
        var wait = new WaitForSeconds(repathInterval);
        while (true)
        {
            if (agent && _player) agent.SetDestination(_player.position);
            yield return wait;
        }
    }

    IEnumerator FindPlayerLoop()
    {
        var wait = new WaitForSeconds(findInterval);
        while (true)
        {
            if (_player == null) _player = FindPlayerByTag();
            else
            {
                // jeœli obiekt gracza zosta³ zniszczony — odœwie¿
                if (!_player.gameObject.activeInHierarchy) _player = FindPlayerByTag();
            }
            yield return wait;
        }
    }

    Transform FindPlayerByTag()
    {
        var objs = GameObject.FindGameObjectsWithTag(playerTag);
        if (objs == null || objs.Length == 0) return null;

        if (!pickNearestIfMany) return objs[0].transform;

        // wybierz najbli¿szego
        Transform best = null;
        float bestDist = float.MaxValue;
        var myPos = transform.position;
        foreach (var go in objs)
        {
            var d = (go.transform.position - myPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = go.transform; }
        }
        return best;
    }

    /// Wywo³ywane przez Trigger 1 (Explode)
    public void TriggerExplodeEntered(Collider2D other)
    {
        if (_isExploded) return;
        if (!other.CompareTag(playerTag)) return;
        ExplodeNow();
    }

    /// W³aœciwy wybuch: 1 dmg, spawn prefabu, znikniêcie
    void ExplodeNow()
    {
        if (_isExploded) return;
        _isExploded = true;

        if (explodeVfxPrefab)
            Instantiate(explodeVfxPrefab, transform.position, Quaternion.identity);

        // Jeœli w chwili wybuchu gracz jest w Trigger 2 -> zadaj 1 dmg
        if (damageZone && damageZone.IsPlayerInside)
        {
            var h = damageZone.LastPlayerHealthInZone;
            if (h == null && damageZone.LastPlayerCollider != null)
                h = damageZone.LastPlayerCollider.GetComponentInParent<Health>();

            if (h != null) h.TakeDamage(1f);
        }

        if (spawnOnDespawnPrefab)
            Instantiate(spawnOnDespawnPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}


