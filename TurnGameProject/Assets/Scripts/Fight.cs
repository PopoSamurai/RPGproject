using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public enum BattleState
{
    start,
    playerTurn,
    enemyTurn,
    won,
    lost,
}
public class Fight : MonoBehaviour
{
    public BattleState state;

    // Dru¿yny
    public CharacterClass playerClass;   // gracz 1
    public CharacterClass playerClass2;  // gracz 2
    public CharacterClass enemyClass;    // wróg 1
    public CharacterClass enemyClass2;   // wróg 2

    // HUD
    public HudPanel playerHud;
    public HudPanel playerHud2;
    public HudPanel enemyHud;
    public HudPanel enemyHud2;

    public Text dialogueText;

    // Który gracz / wróg ma turê (1 lub 2)
    int currentPlayerIndex = 1;
    int currentEnemyIndex = 1;

    // Pozycje startowe graczy
    public Transform playerFirstPos;
    public Transform playerTwoPos;

    // Pozycje wrogów
    public Transform enemyFirstPos;
    public Transform enemyTwoPos;

    // przesuniêcie przy ataku melee
    public Vector3 offset;

    // efekty (0 – magia / pocisk, 2 – uderzenie melee – jak w Twoim starym kodzie)
    public GameObject[] effect;

    // Wywo³aj to, ¿eby zacz¹æ walkê
    public void battleOn()
    {
        state = BattleState.start;
        StartCoroutine(StartBattle());
    }

    // Pod³¹cz pod przycisk "Attack"
    public void AttackButton()
    {
        if (state != BattleState.playerTurn)
            return;

        StartCoroutine(PlayerAttack());
    }

    // Pod³¹cz pod przycisk "Heal"
    public void HealButton()
    {
        if (state != BattleState.playerTurn)
            return;

        StartCoroutine(PlayerHeal());
    }
    IEnumerator StartBattle()
    {
        // Reset kolejek tur
        currentPlayerIndex = 1;
        currentEnemyIndex = 1;

        dialogueText.text = "Rozpoczyna siê walka!";
        yield return new WaitForSeconds(1.5f);

        state = BattleState.playerTurn;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        // Jeœli obaj gracze nie ¿yj¹ – przegrana
        if (!IsAlive(playerClass) && !IsAlive(playerClass2))
        {
            state = BattleState.lost;
            StartCoroutine(Lost());
            yield break;
        }

        // Wybierz aktualnego gracza – jeœli martwy, przeskocz na nastêpnego
        if (currentPlayerIndex == 1 && !IsAlive(playerClass))
            currentPlayerIndex = 2;
        if (currentPlayerIndex == 2 && !IsAlive(playerClass2))
            currentPlayerIndex = 1;

        dialogueText.text = "Twoja tura: " +
                            (currentPlayerIndex == 1 ? playerClass.nameP : playerClass2.nameP);
        yield return null; // czekamy na decyzjê gracza (przyciski)
    }
    IEnumerator EnemyTurn()
    {
        state = BattleState.enemyTurn;
        dialogueText.text = "Tura wroga...";
        yield return new WaitForSeconds(1f);

        // Wróg 1
        if (IsAlive(enemyClass))
        {
            currentEnemyIndex = 1;
            yield return StartCoroutine(EnemyAttack(enemyClass, enemyFirstPos));
        }

        // Wróg 2
        if (IsAlive(enemyClass2))
        {
            currentEnemyIndex = 2;
            yield return StartCoroutine(EnemyAttack(enemyClass2, enemyTwoPos));
        }

        // SprawdŸ, czy gracze ¿yj¹
        if (!IsAlive(playerClass) && !IsAlive(playerClass2))
        {
            state = BattleState.lost;
            StartCoroutine(Lost());
            yield break;
        }

        // Nastêpna tura gracza (zaczyna zawsze gracz 1 jeœli ¿yje)
        currentPlayerIndex = IsAlive(playerClass) ? 1 : 2;
        state = BattleState.playerTurn;
        StartCoroutine(PlayerTurn());
    }
    IEnumerator PlayerAttack()
    {
        CharacterClass attacker = (currentPlayerIndex == 1) ? playerClass : playerClass2;
        HudPanel attackerHud = (currentPlayerIndex == 1) ? playerHud : playerHud2;
        Transform attackerStartPos = (currentPlayerIndex == 1) ? playerFirstPos : playerTwoPos;

        // wybierz pierwszego ¿ywego wroga
        CharacterClass target = IsAlive(enemyClass) ? enemyClass : enemyClass2;
        HudPanel targetHud = (target == enemyClass) ? enemyHud : enemyHud2;
        Transform targetPos = (target == enemyClass) ? enemyFirstPos : enemyTwoPos;

        // brak celu -> wygrana
        if (target == null || !IsAlive(target))
        {
            state = BattleState.won;
            StartCoroutine(Win());
            yield break;
        }

        attacker.AttackCo();

        // melee lub magia
        if (attacker.classChar != CharClass.mage)
        {
            // melee – podejœcie do wroga
            SpriteRenderer sr = attacker.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = attacker.GetComponentInChildren<SpriteRenderer>();

            Vector3 startPos = attackerStartPos.position;
            Vector3 hitPos = targetPos.position + offset;

            // Zmieñ warstwê tylko jeœli obiekt ma SpriteRenderer
            if (sr != null)
                sr.sortingOrder = 3;

            // Podejœcie do celu
            float t = 0f;
            const float moveTime = 0.2f;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                attacker.transform.position = Vector3.Lerp(startPos, hitPos, t / moveTime);
                yield return null;
            }

            // Efekt uderzenia
            Instantiate(effect[2], hitPos, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);

            // Obliczenie trafienia
            bool isDead = target.TakeDamage(attacker.damage);
            targetHud.SetHp(target.currentHp);

            // Powrót
            t = 0f;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                attacker.transform.position = Vector3.Lerp(hitPos, startPos, t / moveTime);
                yield return null;
            }

            // Przywróæ sortingOrder tylko jeœli istnieje SR
            if (sr != null)
                sr.sortingOrder = 2;

            Instantiate(effect[2], hitPos, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);

            dialogueText.text = isDead
                ? "Wróg " + target.nameP + " nie ¿yje!"
                : "Atak siê uda³.";
        }
        else
        {
            // mag – pocisk / czar
            Instantiate(effect[0], targetPos.position, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);

            bool isDead = target.TakeDamage(attacker.damage);
            targetHud.SetHp(target.currentHp);

            dialogueText.text = isDead
                ? "Wróg " + target.nameP + " nie ¿yje!"
                : "Zaklêcie trafia wroga.";
        }

        // ukryj HUD jeœli trup
        if (!IsAlive(enemyClass))
            enemyHud.gameObject.SetActive(false);
        if (!IsAlive(enemyClass2))
            enemyHud2.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.4f);

        // SprawdŸ wygran¹
        if (!IsAlive(enemyClass) && !IsAlive(enemyClass2))
        {
            state = BattleState.won;
            StartCoroutine(Win());
            yield break;
        }

        // Kolejny gracz, jeœli ¿yje
        if (currentPlayerIndex == 1 && IsAlive(playerClass2))
        {
            currentPlayerIndex = 2;
            state = BattleState.playerTurn;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            // przechodzimy do tury wroga
            state = BattleState.enemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerHeal()
    {
        CharacterClass target = (currentPlayerIndex == 1) ? playerClass : playerClass2;
        HudPanel targetHud = (currentPlayerIndex == 1) ? playerHud : playerHud2;

        // Prosty warunek – zmieñ wg potrzeb
        if (target.currentSp >= 10 && target.currentHp < target.health)
        {
            target.Heal(5, 10);
            targetHud.SetHp(target.currentHp);
            targetHud.SetSp(target.currentSp);
            dialogueText.text = "Czujesz przyp³yw zdrowia!";
        }
        else
        {
            dialogueText.text = "Nie jesteœ ranny lub brak many.";
            yield break; // zostajemy w tej samej turze (gracz zmarnowa³ akcjê)
        }

        yield return new WaitForSeconds(0.5f);

        // Po healu – kolejny gracz / wróg tak jak po ataku
        if (currentPlayerIndex == 1 && IsAlive(playerClass2))
        {
            currentPlayerIndex = 2;
            state = BattleState.playerTurn;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            state = BattleState.enemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator EnemyAttack(CharacterClass enemy, Transform enemyPos)
    {
        if (!IsAlive(enemy))
            yield break;

        // wybierz cel – pierwszy ¿ywy gracz
        CharacterClass target = IsAlive(playerClass) ? playerClass : playerClass2;
        HudPanel targetHud = (target == playerClass) ? playerHud : playerHud2;
        Transform targetPos = (target == playerClass) ? playerFirstPos : playerTwoPos;

        if (target == null || !IsAlive(target))
            yield break;

        dialogueText.text = enemy.nameP + " atakuje " + target.nameP + "!";
        yield return new WaitForSeconds(0.3f);

        // prosty melee wroga – efekt na graczu
        Instantiate(effect[2], targetPos.position + offset, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);

        bool isDead = target.TakeDamage(enemy.damage);
        targetHud.SetHp(target.currentHp);

        if (isDead)
        {
            dialogueText.text = target.nameP + " upada!";
            yield return new WaitForSeconds(0.5f);

            if (!IsAlive(playerClass))
                playerHud.gameObject.SetActive(false);
            if (!IsAlive(playerClass2))
                playerHud2.gameObject.SetActive(false);
        }
    }
    IEnumerator Win()
    {
        dialogueText.text = "Zwyciêstwo!";
        yield return new WaitForSeconds(1.5f);

        // Jeœli masz GameManager:
        // FindObjectOfType<BattleSystem.GameManager>().EndBattle();
    }
    IEnumerator Lost()
    {
        dialogueText.text = "Pora¿ka...";
        yield return new WaitForSeconds(1.5f);

        // tu wstaw obs³ugê przegranej (np. ekran game over)
    }
    bool IsAlive(CharacterClass c)
    {
        return c != null && c.currentHp > 0;
    }
}