using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DestroyIfNotTouchingCorridor2D : MonoBehaviour
{
    [Header("Ustawienia")]
    [Tooltip("Tag obiektów reprezentuj¹cych korytarz.")]
    public string corridorTag = "korytarz";

    [Tooltip("Maksymalny czas bez styku (sekundy) zanim obiekt zostanie zniszczony. 0 = natychmiast.")]
    [Min(0f)] public float destroyDelay = 0.05f;

    [Tooltip("Czas po spawnie na 'ustabilizowanie' kolizji zanim zaczniemy egzekwowaæ warunek.")]
    [Min(0f)] public float initialGraceTime = 0.1f;

    [Tooltip("Jeœli collider korytarza jest na dziecku, szukaj tagu równie¿ u rodziców.")]
    public bool checkParentsForTag = true;

    [Tooltip("Obiekt do zniszczenia (domyœlnie ten GameObject).")]
    public GameObject targetToDestroy;

    [Tooltip("Wypisuj logi dla debugowania.")]
    public bool debugLogs = false;

    [Header("Aktywacje przy niszczeniu")]
    [Tooltip("Te obiekty zostan¹ ustawione na aktywne (SetActive(true)) w chwili niszczenia.")]
    public List<GameObject> objectsToActivateOnDestroy = new List<GameObject>();
    [Tooltip("ujawnienie pokojow")]
    public List<GameObject> pokoje = new List<GameObject>();

    private int corridorContacts = 0;
    private float timeWithoutContact = 0f;
    private float lifeTimer = 0f;
    private bool destructionTriggered = false;

    private void Reset()
    {
        corridorTag = "korytarz";

        var rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true; // kolizje Kinematic<->Static
        rb.gravityScale = 0f;

        var col = GetComponent<Collider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = false; // mo¿esz ustawiæ true, wtedy zdarzenia Trigger te¿ zadzia³aj¹
    }

    private void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.useFullKinematicContacts = true;
        rb.gravityScale = 0f;

        if (targetToDestroy == null) targetToDestroy = gameObject;
    }

    private void FixedUpdate()
    {
        if (destructionTriggered) return;

        lifeTimer += Time.fixedDeltaTime;

        if (corridorContacts > 0)
        {
            timeWithoutContact = 0f;
        }
        else
        {
            timeWithoutContact += Time.fixedDeltaTime;

            if (lifeTimer >= initialGraceTime && timeWithoutContact >= destroyDelay)
            {
                TriggerDestroy("brak styku z korytarzem");
            }
        }
    }

    // --- Kolizje (2D) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsCorridor(collision.transform)) corridorContacts++;
        
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsCorridor(collision.transform)) corridorContacts = Mathf.Max(0, corridorContacts - 1);
    }

    // --- Triggery (2D) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsCorridor(other.transform)) corridorContacts++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsCorridor(other.transform)) corridorContacts = Mathf.Max(0, corridorContacts - 1);
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < pokoje.Count; i++)
            {
                if (pokoje[i] != null)
                {
                    pokoje[i].SetActive(true);
                }
            }
        }
    }

    // --- Pomocnicze ---
    private void TriggerDestroy(string reason)
    {
        if (destructionTriggered) return;
        destructionTriggered = true;

        // 1) W³¹cz wskazane obiekty
        if (objectsToActivateOnDestroy != null)
        {
            for (int i = 0; i < objectsToActivateOnDestroy.Count; i++)
            {
                var go = objectsToActivateOnDestroy[i];
                if (go != null && !go.activeSelf)
                {
                    go.SetActive(true);
                }
            }
        }

        if (debugLogs)
            Debug.Log($"[{name}] Niszczenie ({reason}). Aktywowano {objectsToActivateOnDestroy?.Count ?? 0} obiektów.");

        // 2) Zniszcz target
        Destroy(targetToDestroy);

        // 3) Zatrzymaj dzia³anie komponentu
        enabled = false;
    }

    private bool IsCorridor(Transform t)
    {
        if (t == null) return false;

        if (t.CompareTag(corridorTag)) return true;

        if (checkParentsForTag)
        {
            var p = t.parent;
            while (p != null)
            {
                if (p.CompareTag(corridorTag)) return true;
                p = p.parent;
            }
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(corridorTag))
            corridorTag = "korytarz";
    }
#endif
}
