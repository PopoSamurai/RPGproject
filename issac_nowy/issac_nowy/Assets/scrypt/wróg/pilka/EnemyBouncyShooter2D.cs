using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyBouncyShooter2D
/// - Losowy kierunek na starcie
/// - Ruch sta�� pr�dko�ci� i odbijanie od �cian (jak pi�ka)
/// - Strzelanie prefabami z listy "muzzles" co X sekund
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBouncyShooter2D : MonoBehaviour
{
    [Header("Ruch i odbijanie")]
    public float speed = 6f;
    [Tooltip("Warstwy traktowane jako '�ciany' do odbi�.")]
    public LayerMask wallMask;

    [Tooltip("Minimalna pr�dko�� poni�ej kt�rej podbijamy velocity (anty-zawieszenie).")]
    public float minSpeedEpsilon = 0.5f;

    [Header("Strzelanie")]
    public List<Transform> muzzles = new List<Transform>(); // punkty wystrza�u
    public GameObject projectilePrefab;
    [Min(0f)] public float shootInterval = 0.75f;
    [Tooltip("Czy wybiera� losowy muzzle (true), czy cyklicznie (false).")]
    public bool randomMuzzle = true;

    [Header("Losowy start")]
    [Tooltip("Je�li ustawisz, u�yje tej pr�dko�ci startowej zamiast 'speed' (0 = ignoruj).")]
    public float initialSpeedOverride = 0f;
    [Tooltip("Nie zaczynaj z kierunkiem zbyt poziomym/pionowym (0=wy��cz).")]
    [Range(0f, 0.99f)] public float avoidAxisBias = 0.0f;

    Rigidbody2D rb;
    Vector2 lastDir = Vector2.right;
    int muzzleIdx = 0;
    float shootTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    public void Start()
    {
        AddStartingForce();
    }
    public void Update()
    {
        
    }

    
    void FixedUpdate()
    {
        //// Ogranicz prędkość, żeby fizyka się nie rozjechała
        //if (rb.linearVelocity.magnitude > speed && rb.linearVelocity.magnitude > 0.1f)
        //{
        //    rb.linearVelocity = rb.linearVelocity.normalized * speed;
        //}
        //
        //// Strzelanie w odst�pach
        //shootTimer -= Time.fixedDeltaTime;
        //if (shootTimer <= 0f)
        //{
        //    ShootOnce();
        //    shootTimer = shootInterval;
        //}
    }

    //void ShootOnce()
    //{
    //    if (!projectilePrefab || muzzles.Count == 0) return;
    //
    //    foreach (var spawnT in muzzles)
    //    {
    //        if (!spawnT) continue;
    //
    //        // Zachowaj rotację każdego muzzle'a 1:1
    //        Instantiate(projectilePrefab, spawnT.position, spawnT.rotation);
    //    }
    //}

    

    //void OnDrawGizmosSelected()
    //{
    //    // podgl�d kierunku
    //    Gizmos.color = Color.cyan;
    //    Vector3 p = transform.position;
    //    Gizmos.DrawLine(p, p + new Vector3(lastDir.x, lastDir.y, 0f) * 1.0f);
    //}

    private void AddStartingForce()
    {
        float x = Random.value < 0.5f ? -1.0f : 1.0f;
        float y = Random.value < 0.5f ? Random.Range(-1.0f, -0.5f): Random.Range(0.5f, 1.5f);
        Vector2 direction = new Vector2(x, y);
        rb.AddForce(direction * this.speed);
    }
}

