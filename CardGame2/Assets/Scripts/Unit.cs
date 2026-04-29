using System.Collections;
using UnityEngine;
public class Unit : MonoBehaviour
{
    public CardData sourceCard;
    public int maxHP;
    public int currentHP;
    public int attack; 
    public int counter;
    public bool IsReady;
    public bool isDead = false;

    public bool isLeader;
    public SlotOwner owner;
    public BoardSlot CurrentSlot;

    public bool IsAnimating = false;
    public void Tick()
    {
        if (counter > 1)
        {
            counter--;
        }
        else if (counter == 1)
        {
            IsReady = true;
        }
        var view = GetComponent<CardView>();
        if (view != null)
        {
            view.UpdateStatsUI();
        }
    }
    public void ResetCounter()
    {
        if (sourceCard != null)
        {
            counter = sourceCard.couner;
            IsReady = false;

            var view = GetComponent<CardView>();
            if (view != null)
                view.UpdateStatsUI();
        }
    }
    private void Start()
    {
        IsReady = false;
    }
    public void Init(CardData data)
    {
        sourceCard = data;
        maxHP = data.hp;
        currentHP = data.hp;
        attack = data.attack;

        if (data.type == CardType.Character)
        {
            counter = data.couner;
            IsReady = false;
        }
    }
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        SpawnDamageText(dmg);

        if (currentHP <= 0)
            Die();

        var view = GetComponent<CardView>();
        if (view != null)
        {
            view.UpdateStatsUI();
        }
    }
    void SpawnDamageText(int dmg)
    {
        Debug.Log($"DAMAGE POPUP: {dmg}");
    }
    public void Heal(int amount)
    {
        currentHP += amount;
    }
    public void AddAttack(int amount)
    {
        attack += amount;
    }
    public IEnumerator PerformAttackRoutine()
    {
        IsAnimating = true;
        var attackerView = GetComponent<CardView>();
        var attackerRT = attackerView.GetComponent<RectTransform>();

        Transform originalParent = attackerRT.parent;
        Vector3 originalScale = attackerRT.localScale;
        try
        {
            var target = BoardSystem.Instance.GetFrontTarget(CurrentSlot);

            if (target == null)
                yield break;

            var targetView = target.GetComponent<CardView>();
            RectTransform targetRT = targetView.GetComponent<RectTransform>();

            var rootCanvas = FindObjectOfType<Canvas>();
            attackerRT.SetParent(rootCanvas.transform, true);
            attackerRT.SetAsLastSibling();

            Vector3 startPos = attackerRT.position;
            Vector3 dir = (targetRT.position - attackerRT.position).normalized;

            float attackerWidth = attackerRT.rect.width * attackerRT.lossyScale.x;
            float targetWidth = targetRT.rect.width * targetRT.lossyScale.x;

            float spacing = 20f;
            float stopDistance = (attackerWidth + targetWidth) / 2f + spacing;
            Vector3 targetPos = targetRT.position - dir * stopDistance;

            float t = 0;
            float duration = 0.12f;

            while (t < duration * 0.8f)
            {
                t += Time.deltaTime;
                float eased = Mathf.Sin((t / duration) * Mathf.PI * 0.5f);
                attackerRT.position = Vector3.Lerp(startPos, targetPos, eased);
                yield return null;
            }
            Vector3 enemyStart = targetRT.position;
            target.TakeDamage(attack);
            StartCoroutine(BattleSystem.Instance.ScreenShake(0.08f, 5f));

            attackerRT.localScale = originalScale * 1.15f;
            yield return new WaitForSeconds(0.04f);
            attackerRT.localScale = originalScale;

            if (targetView != null)
            {
                StartCoroutine(targetView.FlashHP());
                if (targetRT != null)
                {
                    Vector3 originalTargetScale = targetRT.localScale;

                    targetRT.localScale = originalTargetScale * 1.1f;
                    yield return new WaitForSeconds(0.05f);
                    targetRT.localScale = originalTargetScale;
                }
            }
            //efect
            if (BattleSystem.Instance.hitEffectPrefab != null)
            {
                GameObject fx = Instantiate(
                    BattleSystem.Instance.hitEffectPrefab,
                    targetRT.position,
                    Quaternion.identity,
                    rootCanvas.transform
                );

                RectTransform fxRT = fx.GetComponent<RectTransform>();
                fxRT.position = targetRT.position;

                Destroy(fx, 0.5f);
            }
            StartCoroutine(HitPause());
            bool targetAlive = target != null && !target.isDead;

            if (targetAlive && targetRT != null)
            {
                float direction = (owner == SlotOwner.Player) ? 1f : -1f;
                Vector3 enemyBack = enemyStart + Vector3.right * 30f * direction;

                t = 0;
                while (t < 0.06f)
                {
                    if (targetRT == null) break;

                    t += Time.deltaTime;
                    targetRT.position = Vector3.Lerp(enemyStart, enemyBack, t / 0.06f);
                    yield return null;
                }

                t = 0;
                while (t < 0.06f)
                {
                    if (targetRT == null) break;

                    t += Time.deltaTime;
                    targetRT.position = Vector3.Lerp(enemyBack, enemyStart, t / 0.06f);
                    yield return null;
                }
            }
            attackerRT.SetParent(originalParent, false);
            attackerRT.localScale = originalScale;
            attackerRT.localPosition = Vector3.zero;
            attackerRT.localRotation = Quaternion.identity;
        }
        finally
        {
            if (attackerRT != null && originalParent != null)
            {
                attackerRT.SetParent(originalParent, false);
                attackerRT.localScale = originalScale;
                attackerRT.localPosition = Vector3.zero;
                attackerRT.localRotation = Quaternion.identity;
            }
            IsAnimating = false;
        }
    }
    IEnumerator HitPause()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1f;
    }
    public IEnumerator HitPush(Vector3 attackerPos)
    {
        Vector3 original = transform.position;
        Vector3 dir = (transform.position - attackerPos).normalized;
        Vector3 hitPos = original + dir * 0.15f;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            transform.position = Vector3.Lerp(original, hitPos, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            transform.position = Vector3.Lerp(hitPos, original, t);
            yield return null;
        }
    }
    public IEnumerator Shake()
    {
        Vector3 original = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < 0.2f)
        {
            float x = Random.Range(-0.05f, 0.05f);
            float y = Random.Range(-0.05f, 0.05f);

            transform.localPosition = original + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = original;
    }
    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (isLeader)
        {
            BattleSystem.Instance.OnLeaderDied(this);
        }
        StartCoroutine(DieRoutine());
    }
    IEnumerator DieRoutine()
    {
        IsAnimating = true;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        cg.blocksRaycasts = false;

        var card = GetComponent<CardView>();
        var rt = GetComponent<RectTransform>();

        Transform originalParent = rt.parent;
        var rootCanvas = FindObjectOfType<Canvas>();

        rt.SetParent(rootCanvas.transform, true);
        rt.SetAsLastSibling();

        Vector3 attackerPos = BattleSystem.Instance.LastAttackerPosition;
        yield return StartCoroutine(DeathFlyAnimation(rt, attackerPos));
        if (card != null && card.CurrentSlot != null)
        {
            var slot = card.CurrentSlot;
            slot.line?.HandleUnitDeath(slot.indexInLine);
            slot.line?.Collapse();
        }
        IsAnimating = false;
        Destroy(gameObject);
    }
    IEnumerator DeathFlyAnimation(RectTransform rt, Vector3 attackerPosition)
    {
        Vector3 start = rt.position;

        float liftHeight = 250f;
        float fallDistance = 800f;
        Vector3 attackDir = (start - attackerPosition).normalized;

        float sideMultiplier = (owner == SlotOwner.Player) ? -1f : 1f;
        attackDir.x = Mathf.Abs(attackDir.x) * sideMultiplier;
        attackDir = attackDir.normalized;

        Vector3 upTarget = start + Vector3.up * liftHeight;
        Vector3 end = start + attackDir * fallDistance;

        float t = 0f;
        float fallDuration = 0.9f;

        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float p = t / fallDuration;

            float eased = Mathf.Pow(p, 1.6f);
            Vector3 pos = Vector3.Lerp(upTarget, end, eased);
            float arc = Mathf.Sin(p * Mathf.PI) * 200f;
            pos.y += arc;
            rt.position = pos;

            float spin = 120f + eased * 320f;
            rt.rotation = Quaternion.Euler(0, 0, spin * Mathf.Sign(attackDir.x));

            yield return null;
        }
        CanvasGroup cg = rt.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = rt.gameObject.AddComponent<CanvasGroup>();

        float fadeT = 0f;
        while (fadeT < 0.1f)
        {
            fadeT += Time.deltaTime;
            cg.alpha = 1f - (fadeT / 0.1f);
            yield return null;
        }
    }
}