using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BattleUnit : MonoBehaviour
{
    [Header("UI")]
    public Image hpBarFillImage;
    public Image elementIconImage; //element
    public Text shieldText;

    [Header("Weakness Icons")]
    public Image[] weaknessIconSlots;
    public ElementIconSet elementIconSet;

    [Header("Step out")]
    public float stepOutOffsetX = 0.5f;
    public float stepOutSpeed = 6f;
    private Vector3 _stepOutPosition;

    public int CurrentShield { get; private set; }
    public bool HasShield => data != null && data.maxShield > 0;
    public bool IsBroken { get; private set; }

    public CharacterData data;
    public BattlePosition position;
    public int CurrentHP { get; private set; }
    public bool IsDead => CurrentHP <= 0;
    public bool IsPlayer => data != null && data.isPlayer;
    public bool IsDefending { get; private set; }
    [Header("Ruch")]
    public float moveSpeed = 6f;
    float attackOffsetFromTarget = 0.2f;

    [Header("Hit effect")]
    public Material hitMaterial;
    private Material _defaultMaterial;
    private static readonly int FlashPropId = Shader.PropertyToID("_Flash");

    private Vector3 _startPosition;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine highlightRoutine;

    private void SetFlash(float value)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.material.SetFloat(FlashPropId, value);
    }
    void SetupElementIcon()
    {
        if (elementIconImage == null || elementIconSet == null || data == null)
            return;

        ElementType elem = data.attackElement;
        if (elem == ElementType.None)
        {
            elementIconImage.gameObject.SetActive(false);
            return;
        }

        Sprite icon = elementIconSet.GetIcon(elem);
        elementIconImage.sprite = icon;
        elementIconImage.preserveAspect = true;
        elementIconImage.gameObject.SetActive(true);
    }
    void UpdateShieldUI()
    {
        if (shieldText == null)
            return;

        Transform root = shieldText.transform.parent;
        if (!HasShield || CurrentShield <= 0)
        {
            if (root != null)
                root.gameObject.SetActive(false);

            return;
        }
        if (root != null)
            root.gameObject.SetActive(true);

        shieldText.text = CurrentShield.ToString();
    }
    bool IsHitWeakness(ElementType element)
    {
        if (element == ElementType.None || data == null || data.elementalWeaknesses == null)
            return false;

        foreach (var weak in data.elementalWeaknesses)
        {
            if (weak == element)
                return true;
        }
        return false;
    }
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
    void SetupWeaknessIcons()
    {
        if (weaknessIconSlots == null || elementIconSet == null || data == null)
            return;

        for (int i = 0; i < weaknessIconSlots.Length; i++)
        {
            if (weaknessIconSlots[i] != null)
                weaknessIconSlots[i].gameObject.SetActive(false);
        }
        if (data.elementalWeaknesses == null)
            return;

        int count = Mathf.Min(data.elementalWeaknesses.Length, weaknessIconSlots.Length);
        if (count == 0)
            return;

        float spacing = 20f;

        for (int i = 0; i < count; i++)
        {
            var slot = weaknessIconSlots[i];
            if (slot == null) continue;

            ElementType elem = data.elementalWeaknesses[i];
            Sprite icon = elementIconSet.GetIcon(elem);

            slot.sprite = icon;
            slot.preserveAspect = true;
            slot.gameObject.SetActive(true);

            RectTransform rt = slot.rectTransform;
            Vector2 pos = rt.anchoredPosition;

            float indexFromCenter = i - (count - 1) * 0.5f;
            pos.x = indexFromCenter * spacing;

            rt.anchoredPosition = pos;
        }
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

            if (hitMaterial != null)
            {
                _spriteRenderer.material = new Material(hitMaterial);
            }

            _defaultMaterial = _spriteRenderer.material;
            SetFlash(0f);
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
        IsBroken = false;
        IsDefending = false;

        CurrentHP = data.maxHP;
        CurrentShield = data.maxShield;

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (data.sprite != null)
            _spriteRenderer.sprite = data.sprite;

        SetupElementIcon();
        SetupWeaknessIcons();
        UpdateHpBar();
        UpdateShieldUI();
    }
    public void ReceiveDamage(int amount, ElementType element = ElementType.None)
    {
        int prevHP = CurrentHP;
        int finalAmount = amount;
        bool hitWeakness = IsHitWeakness(element);
        bool wasBroken = IsBroken;

        if (hitWeakness && HasShield && CurrentShield > 0)
        {
            CurrentShield -= 1;
            if (CurrentShield < 0) CurrentShield = 0;
            if (CurrentShield == 0 && !IsBroken)
            {
                IsBroken = true;
                if (DamageTextManager.Instance != null)
                {
                    Vector3 offset = Vector3.up * 0.3f;
                    bool fromPlayerSide = data.isPlayer;
                    DamageTextManager.Instance.ShowDamageText(
                        -1,
                        transform.position + offset,
                        false,
                        fromPlayerSide
                    );
                }
            }
            UpdateShieldUI();
        }
        // DEFEND
        if (IsDefending)
        {
            finalAmount = Mathf.CeilToInt(finalAmount * 0.5f);
            IsDefending = false;
        }
        float elementMult = GetElementMultiplier(element);
        finalAmount = Mathf.CeilToInt(finalAmount * elementMult);

        if (wasBroken)
            finalAmount = Mathf.CeilToInt(finalAmount * 2f);

        CurrentHP -= finalAmount;
        if (CurrentHP < 0) CurrentHP = 0;
        int actualDamage = prevHP - CurrentHP;

        if (actualDamage > 0 && DamageTextManager.Instance != null)
        {
            Vector3 offset = Vector3.up * 0.1f;
            bool fromPlayerSide = data.isPlayer;
            DamageTextManager.Instance.ShowDamageText(actualDamage, transform.position + offset, false, fromPlayerSide);
        }

        if (CameraShake.Instance != null && actualDamage > 0)
            CameraShake.Instance.Shake(0.08f, 0.05f);

        Debug.Log($"{data.characterName} otrzymuje {actualDamage} DMG (HP={CurrentHP})");
        UpdateHpBar();

        StartCoroutine(HitFlash());
        StartCoroutine(HitPushback());

        if (CurrentHP <= 0)
            StartCoroutine(Die());
    }
    public void OnBrokenTurn()
    {
        Debug.Log($"{data.characterName} jest oszołomiony (BREAK) i pomija turę.");
        if (HasShield)
        {
            CurrentShield = data.maxShield;
            UpdateShieldUI();
        }
        IsBroken = false;
    }
    float GetElementMultiplier(ElementType element)
    {
        if (element == ElementType.None || data == null)
            return 1f;

        // Vulnerabilities
        if (data.elementalWeaknesses != null)
        {
            for (int i = 0; i < data.elementalWeaknesses.Length; i++)
            {
                if (data.elementalWeaknesses[i] == element)
                    return 1.5f; // +50% dmg
            }
        }

        // Ressist
        if (data.elementalResistances != null)
        {
            for (int i = 0; i < data.elementalResistances.Length; i++)
            {
                if (data.elementalResistances[i] == element)
                    return 0.5f; // half dmg
            }
        }

        return 1f;
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
        SetFlash(0f);

        if (_spriteRenderer != null)
            _spriteRenderer.color = _originalColor;
    }
    IEnumerator HighlightBlink()
    {
        if (_spriteRenderer == null) yield break;

        float t = 0f;
        float speed = 4f;
        float maxFlash = 0.10f;

        while (true)
        {
            t += Time.deltaTime * speed;
            float pulse = 0.5f + 0.5f * Mathf.Sin(t);
            float amount = pulse * maxFlash;

            SetFlash(amount);
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

        int flashes = 1;
        float singleFlashTime = 0.12f;
        float maxFlash = 1.0f;
        float flashSpeed = 12f;

        for (int i = 0; i < flashes; i++)
        {
            float t = 0f;
            while (t < singleFlashTime)
            {
                t += Time.deltaTime;
                float lerp = Mathf.PingPong(t * flashSpeed, 1f);
                float amount = Mathf.Lerp(0f, maxFlash, lerp);

                SetFlash(amount);
                yield return null;
            }
        }

        SetFlash(0f);
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
    public void Defend()
    {
        IsDefending = true;
        Debug.Log($"{data.characterName} przyjmuje pozycję obronną (Defend).");
        // tu możesz później dodać animację, efekt itp.
    }
    public void Heal(int amount)
    {
        int prevHP = CurrentHP;
        int missing = data.maxHP - CurrentHP;
        int actualHeal = Mathf.Clamp(amount, 0, missing);
        int shownHeal = Mathf.Max(actualHeal, 0);

        if (DamageTextManager.Instance != null)
        {
            Vector3 offset = Vector3.up * 0.5f;
            bool fromPlayerSide = data.isPlayer;
            DamageTextManager.Instance.ShowDamageText(
                shownHeal,
                transform.position + offset,
                true,
                fromPlayerSide
            );
        }
        if (actualHeal <= 0)
        {
            Debug.Log($"{data.characterName} nie potrzebuje leczenia (HP={CurrentHP}), heal = 0");
            return;
        }
        CurrentHP += actualHeal;
        Debug.Log($"{data.characterName} leczy {actualHeal} HP (HP={CurrentHP})");
        UpdateHpBar();
    }
}