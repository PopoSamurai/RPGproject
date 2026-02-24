using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimedDamageZone2D : MonoBehaviour
{
    [Header("Konfiguracja")]
    [Tooltip("Tag obiektów, które maj¹ dostawaæ obra¿enia (np. Player).")]
    public string targetTag = "Player";

    [Tooltip("Co ile sekund zadaæ obra¿enia (X).")]
    [Min(0.01f)] public float damageInterval = 1.0f;

    [Tooltip("Ile sekund po pierwszym wejœciu strefa znika (Z).")]
    [Min(0.01f)] public float lifetimeAfterFirstEnter = 5.0f;

    [Tooltip("Ile obra¿eñ na tick (domyœlnie 1).")]
    public float damagePerTick = 1f;

    [Header("Opcje")]
    [Tooltip("Czy zacz¹æ licznik znikniêcia natychmiast po starcie (zamiast po 1. wejœciu)?")]
    public bool startLifetimeOnAwake = false;

    // wewnêtrzne
    private bool _lifetimeStarted = false;
    private readonly Dictionary<Health, Coroutine> _runningTicks = new Dictionary<Health, Coroutine>();

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Awake()
    {
        if (startLifetimeOnAwake && !_lifetimeStarted)
        {
            _lifetimeStarted = true;
            StartCoroutine(CoSelfDestructAfter(lifetimeAfterFirstEnter));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        // uruchom licznik ¿ycia po pierwszym kontakcie
        if (!_lifetimeStarted && !startLifetimeOnAwake)
        {
            _lifetimeStarted = true;
            StartCoroutine(CoSelfDestructAfter(lifetimeAfterFirstEnter));
        }

        var health = other.GetComponentInParent<Health>();
        if (health == null) return;

        // jeden ticker per Health
        if (_runningTicks.ContainsKey(health)) return;

        var co = StartCoroutine(CoDamageTick(health));
        _runningTicks.Add(health, co);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        var health = other.GetComponentInParent<Health>();
        if (health == null) return;

        if (_runningTicks.TryGetValue(health, out var co))
        {
            StopCoroutine(co);
            _runningTicks.Remove(health);
        }
    }

    IEnumerator CoDamageTick(Health target)
    {
        // Zadaj natychmiast pierwszy tick? Jeœli tak – odkomentuj:
        // target.TakeDamage(damagePerTick);

        var wait = new WaitForSeconds(damageInterval);
        while (target != null && target.IsAlive)
        {
            target.TakeDamage(damagePerTick);
            yield return wait;
        }

        // sprz¹tanie, gdy target umar³/znikn¹³ w trakcie
        if (target != null) _runningTicks.Remove(target);
    }

    IEnumerator CoSelfDestructAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // zatrzymaj wszystkie tikery
        foreach (var kv in _runningTicks)
            if (kv.Value != null) StopCoroutine(kv.Value);
        _runningTicks.Clear();

        Destroy(gameObject);
    }

    void OnDisable()
    {
        // bezpieczeñstwo: zatrzymaj wszystko, gdy obiekt jest wy³¹czany
        foreach (var kv in _runningTicks)
            if (kv.Value != null) StopCoroutine(kv.Value);
        _runningTicks.Clear();
    }
}
