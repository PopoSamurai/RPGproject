using System.Collections;
using UnityEngine;
public class Unit : MonoBehaviour
{
    public CardData sourceCard;
    public int maxHP;
    public int currentHP;
    public int attack; 
    public int counter;
    public int baseCounter = 3;
    public bool IsReady;
    public bool isDead = false;

    public bool isLeader;
    public SlotOwner owner;
    public BoardSlot CurrentSlot;
    public void Tick()
    {
        counter--;

        if (counter <= 0)
        {
            counter = 0;
            IsReady = true;
        }

        Debug.Log($"[TICK] {name} counter={counter}, ready={IsReady}");
    }
    private void Start()
    {
        IsReady = false;
        counter = baseCounter;
    }
    public void Init(CardData data)
    {
        sourceCard = data;
        maxHP = data.hp;
        currentHP = data.hp;
        attack = data.attack;
    }
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"{name} took {dmg} damage");
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
        Debug.Log($"[UNIT] {name} healed {amount}, HP = {currentHP}");
    }
    public void AddAttack(int amount)
    {
        attack += amount;
        Debug.Log("Dodaj atak" + attack);
    }
    public IEnumerator PerformAttackRoutine()
    {
        var attackerView = GetComponent<CardView>();
        var target = BoardSystem.Instance.GetFrontTarget(CurrentSlot);

        if (target == null)
            yield break;

        var targetView = target.GetComponent<CardView>();

        RectTransform attackerRT = attackerView.GetComponent<RectTransform>();
        RectTransform targetRT = targetView.GetComponent<RectTransform>();
        Transform originalParent = attackerRT.parent;
        Vector3 originalScale = attackerRT.localScale;

        var rootCanvas = FindObjectOfType<Canvas>();
        attackerRT.SetParent(rootCanvas.transform, true);
        attackerRT.SetAsLastSibling();

        Vector3 startPos = attackerRT.position;
        Vector3 dir = (targetRT.position - attackerRT.position).normalized;
        float stopDistance = 80f;

        Vector3 targetPos = targetRT.position - dir * stopDistance;

        float t = 0;
        float duration = 0.2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            attackerRT.position = Vector3.Lerp(startPos, targetPos, t / duration);
            yield return null;
        }
        target.TakeDamage(attack);
        if (targetView != null)
        {
            StartCoroutine(targetView.FlashHP());
        }
        //efect
        var canvas = FindObjectOfType<Canvas>();

        if (BattleSystem.Instance.hitEffectPrefab != null)
        {
            GameObject fx = Instantiate(
                BattleSystem.Instance.hitEffectPrefab,
                targetRT.position,
                Quaternion.identity,
                canvas.transform
            );

            RectTransform fxRT = fx.GetComponent<RectTransform>();
            fxRT.position = targetRT.position;

            Destroy(fx, 0.5f);
        }
        StartCoroutine(HitPause());
        Vector3 enemyStart = targetRT.position;
        float direction = (owner == SlotOwner.Player) ? 1f : -1f;
        Vector3 enemyBack = enemyStart + Vector3.right * 30f * direction;
        t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            targetRT.position = Vector3.Lerp(enemyStart, enemyBack, t / 0.1f);
            yield return null;
        }
        t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            targetRT.position = Vector3.Lerp(enemyBack, enemyStart, t / 0.1f);
            yield return null;
        }
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            attackerRT.position = Vector3.Lerp(targetPos, startPos, t / duration);
            yield return null;
        }
        attackerRT.SetParent(originalParent, false);
        attackerRT.localScale = originalScale;
        attackerRT.localPosition = Vector3.zero;
        attackerRT.localRotation = Quaternion.identity;
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
        yield return new WaitForSeconds(0.2f);

        var card = GetComponent<CardView>();

        if (card != null && card.CurrentSlot != null)
        {
            var slot = card.CurrentSlot;
            slot.line?.HandleUnitDeath(slot.indexInLine);
            slot.line?.Collapse();
        }

        Destroy(gameObject);
    }
}