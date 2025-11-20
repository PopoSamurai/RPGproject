using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
public class BattleUnit : MonoBehaviour
{
    [Header("UI")]
    public Image hpBarFillImage;

    [Header("Step out")]
    public float stepOutOffsetX = 0.5f;
    public float stepOutSpeed = 6f;
    private Vector3 _stepOutPosition;

    public CharacterData data;
    public BattlePosition position;
    public int CurrentHP { get; private set; }
    public bool IsDead => CurrentHP <= 0;
    public bool IsPlayer => data != null && data.isPlayer;
    [Header("Ruch")]
    public float moveSpeed = 6f;
    float attackOffsetFromTarget = 0.2f;
    private Coroutine highlightRoutine;

    [Header("Hit effect")]
    public Material hitMaterial;
    private Material _defaultMaterial;
    private Vector3 _startPosition;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    void UpdateHpBar()
    {
        if (hpBarFillImage == null || data == null) return;

        float normalized = (float)CurrentHP / data.maxHP;
        hpBarFillImage.fillAmount = Mathf.Clamp01(normalized);
    }
    public IEnumerator StepOut()
    {
        while (Vector3.Distance(transform.position, _stepOutPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _stepOutPosition,
                stepOutSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _stepOutPosition;
    }
    public IEnumerator HighlightTarget(float time = 0.3f)
    {
        if (_spriteRenderer == null) yield break;

        Color original = _spriteRenderer.color;
        Color highlight = Color.yellow;

        _spriteRenderer.color = highlight;
        yield return new WaitForSeconds(time);
        _spriteRenderer.color = original;
    }
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
            _defaultMaterial = _spriteRenderer.material;
        }
        if (hpBarFillImage == null)
        {
            var images = GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                if (img.gameObject.name == "Image_Fill")
                {
                    hpBarFillImage = img;
                    break;
                }
            }
        }
    }
    public void Init(BattlePosition pos, CharacterData characterData)
    {
        data = characterData;
        position = pos;
        pos.currentUnit = this;

        _startPosition = pos.transform.position;
        if (pos.stepOutPoint != null)
        {
            _stepOutPosition = pos.stepOutPoint.position;
        }
        else
        {
            _stepOutPosition = _startPosition + new Vector3(
                IsPlayer ? stepOutOffsetX : -stepOutOffsetX,
                0, 0);
        }
        transform.position = _startPosition;

        CurrentHP = data.maxHP;

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (data.sprite != null)
            _spriteRenderer.sprite = data.sprite;

        UpdateHpBar();
    }
    public void ReceiveDamage(int amount)
    {
        int prevHP = CurrentHP;
        CurrentHP -= amount;
        if (CurrentHP < 0) CurrentHP = 0;
        int actualDamage = prevHP - CurrentHP;

        if (actualDamage > 0 && DamageTextManager.Instance != null)
        {
            Vector3 offset = Vector3.up * 0.5f;
            bool fromPlayerSide = data.isPlayer;
            DamageTextManager.Instance.ShowDamageText(actualDamage, transform.position + offset, false, fromPlayerSide);
        }
        if (CameraShake.Instance != null && actualDamage > 0)
        {
            CameraShake.Instance.Shake(0.08f, 0.05f);
        }
        Debug.Log($"{data.characterName} otrzymuje {actualDamage} DMG (HP={CurrentHP})");
        UpdateHpBar();

        StartCoroutine(HitFlash());
        StartCoroutine(HitPushback());

        if (CurrentHP <= 0)
        {
            StartCoroutine(Die());
        }
    }
    public void StartHighlight()
    {
        if (highlightRoutine != null)
            StopCoroutine(highlightRoutine);

        highlightRoutine = StartCoroutine(HighlightBlink());
    }
    public void StopHighlight()
    {
        if (highlightRoutine != null)
        {
            StopCoroutine(highlightRoutine);
            highlightRoutine = null;
        }

        if (_spriteRenderer != null)
        {
            if (_defaultMaterial != null)
                _spriteRenderer.material = _defaultMaterial;

            _spriteRenderer.color = _originalColor;
        }
    }
    IEnumerator HighlightBlink()
    {
        if (_spriteRenderer == null) yield break;

        float t = 0f;
        float whiteAmount = 0.45f;
        float speed = 4f;

        while (true)
        {
            t += Time.deltaTime * speed;
            float pulse = 0.5f + 0.5f * Mathf.Sin(t);

            float lerpVal = pulse * whiteAmount;
            Color target = Color.Lerp(_originalColor, Color.red, lerpVal);

            _spriteRenderer.color = target;
            yield return null;
        }
    }
    IEnumerator HitPushback(float distance = 0.2f, float speed = 6f)
    {
        Vector3 originalPos = transform.position;
        BattleUnit attacker = BattleManager.Instance.LastAttacker;
        Vector3 dir = (originalPos - attacker.transform.position).normalized;
        if (dir == Vector3.zero)
            dir = Vector3.right;

        Vector3 pushedPos = originalPos + dir * distance;
        while (Vector3.Distance(transform.position, pushedPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                pushedPos,
                speed * Time.deltaTime
            );
            yield return null;
        }
        while (Vector3.Distance(transform.position, originalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originalPos,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }
    public IEnumerator HitFlash()
    {
        if (_spriteRenderer == null) yield break;

        int flashes = 2;
        float singleFlashTime = 0.08f;
        float whiteAmount = 0.7f;

        for (int i = 0; i < flashes; i++)
        {
            float t = 0f;
            while (t < singleFlashTime)
            {
                t += Time.deltaTime;
                float lerp = Mathf.PingPong(t * 10f, 1f);
                Color c = Color.Lerp(_originalColor, Color.red, lerp * whiteAmount);
                _spriteRenderer.color = c;
                yield return null;
            }
            _spriteRenderer.color = _originalColor;
        }
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.15f);
        if (hpBarFillImage != null)
            hpBarFillImage.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    public IEnumerator MoveToPosition(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime);

            yield return null;
        }
        transform.position = targetPos;
    }
    public IEnumerator MoveToAttackPosition(Transform target)
    {
        Vector3 dir = (target.position - _startPosition).normalized;
        Vector3 attackPos = target.position - dir * attackOffsetFromTarget;
        yield return MoveToPosition(attackPos);
    }
    public IEnumerator ReturnToStart()
    {
        yield return MoveToPosition(_startPosition);
    }
    public void Heal(int amount)
    {
        int prevHP = CurrentHP;
        int missing = data.maxHP - CurrentHP;
        int actualHeal = Mathf.Clamp(amount, 0, missing);
        if (actualHeal <= 0)
        {
            Debug.Log($"{data.characterName} nie potrzebuje leczenia (HP={CurrentHP})");
            return;
        }
        CurrentHP += actualHeal;

        if (DamageTextManager.Instance != null)
        {
            Vector3 offset = Vector3.up * 0.5f;
            bool fromPlayerSide = data.isPlayer;
            DamageTextManager.Instance.ShowDamageText(actualHeal, transform.position + offset, true, fromPlayerSide);
        }
        Debug.Log($"{data.characterName} leczy {actualHeal} HP (HP={CurrentHP})");
        UpdateHpBar();
    }
}