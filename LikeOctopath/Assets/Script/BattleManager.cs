using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum BattleActionType { Attack, Heal, Defend, Skip }
public class BattleManager : MonoBehaviour
{
    private List<BattleUnit> _turnQueue = new List<BattleUnit>();
    public static BattleManager Instance;
    [HideInInspector]
    public BattleUnit LastAttacker;

    [Header("UI")]
    public Text turnText;
    public GameObject actionButtonsPanel;
    public GameObject targetButtonsPanel;
    public Button[] targetButtons;
    float enemyDecisionDelay = 1f;
    public TurnOrderUI turnOrderUI;

    [Header("Ustaw w inspectorze")]
    public BattlePosition[] positions;
    public BattleUnit unitPrefab;

    public CharacterData[] playerCharacters;
    public CharacterData[] enemyCharacters;

    private List<BattleUnit> _allUnits = new List<BattleUnit>();
    private List<BattleUnit> _enemyUnits = new List<BattleUnit>();
    private List<BattleUnit> _playerUnits = new List<BattleUnit>();
    private BattleUnit _currentUnit;

    private BattleActionType chosenPlayerAction;

    private bool waitingForActionChoice;
    private bool waitingForTargetChoice;
    private int chosenTargetIndex;
    private BattleUnit _highlightedUnit;
    private bool playerCanceledTargetSelection;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SpawnUnits();

        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
        if (targetButtonsPanel != null) targetButtonsPanel.SetActive(false);

        StartCoroutine(BattleLoop());
        UpdateEnemyTargetButtons();
        UpdateAllyTargetButtons();
    }
    void UpdateEnemyTargetButtons()
    {
        if (targetButtons == null) return;

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (targetButtons[i] == null) continue;

            bool alive = i < _enemyUnits.Count &&
                         _enemyUnits[i] != null &&
                         !_enemyUnits[i].IsDead &&
                         _enemyUnits[i].gameObject.activeSelf;

            targetButtons[i].gameObject.SetActive(alive);
        }
    }

    void UpdateAllyTargetButtons()
    {
        if (targetButtons == null) return;

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (targetButtons[i] == null) continue;

            bool alive = i < _playerUnits.Count &&
                         _playerUnits[i] != null &&
                         !_playerUnits[i].IsDead &&
                         _playerUnits[i].gameObject.activeSelf;

            targetButtons[i].gameObject.SetActive(alive);
        }
    }
    void SpawnUnits()
    {
        int pIndex = 0;
        int eIndex = 0;

        foreach (var pos in positions)
        {
            if (pos.isPlayerSlot)
            {
                if (pIndex >= playerCharacters.Length) continue;
                var unit = Instantiate(unitPrefab);
                unit.Init(pos, playerCharacters[pIndex]);
                _allUnits.Add(unit);
                _playerUnits.Add(unit);
                pIndex++;
            }
            else
            {
                if (eIndex >= enemyCharacters.Length) continue;
                var unit = Instantiate(unitPrefab);
                unit.Init(pos, enemyCharacters[eIndex]);
                _allUnits.Add(unit);
                _enemyUnits.Add(unit);
                eIndex++;
            }
        }
        UpdateEnemyTargetButtons();
        UpdateAllyTargetButtons();
    }
    IEnumerator BattleLoop()
    {
        while (true)
        {
            if (IsTeamDead(true))
            {
                Debug.Log("Przegrana - drużyna padła");
                yield break;
            }
            if (IsTeamDead(false))
            {
                Debug.Log("Wygrana - wszyscy wrogowie pokonani");
                yield break;
            }

            var turnOrder = _allUnits
                .Where(u => !u.IsDead)
                .OrderByDescending(u => u.data.speed)
                .ToList();

            _turnQueue = new List<BattleUnit>(turnOrder);

            for (int i = 0; i < turnOrder.Count; i++)
            {
                var unit = turnOrder[i];
                if (unit.IsDead) continue;

                _currentUnit = unit;
                if (turnOrderUI != null)
                    turnOrderUI.Refresh(_turnQueue, _currentUnit);

                Debug.Log($"Tura: {_currentUnit.data.characterName}");
                yield return StartCoroutine(HandleTurn(_currentUnit));

                if (_turnQueue.Count > 0)
                {
                    var first = _turnQueue[0];
                    _turnQueue.RemoveAt(0);

                    if (first != null && !first.IsDead)
                    {
                        _turnQueue.Add(first);
                    }
                    else
                    {
                        _turnQueue = _turnQueue
                            .Where(u => u != null && !u.IsDead)
                            .ToList();
                    }
                }
            }
            IEnumerator PerformAttack(BattleUnit attacker, BattleUnit target)
            {
                if (target == null || target.IsDead) yield break;
                Debug.Log($"{attacker.data.characterName} atakuje {target.data.characterName}");
                yield return StartCoroutine(attacker.StepOut());

                LastAttacker = attacker;
                target.ReceiveDamage(attacker.data.attackPower, attacker.data.attackElement);

                yield return new WaitForSeconds(0.2f);

                UpdateEnemyTargetButtons();
                UpdateAllyTargetButtons();

                yield return StartCoroutine(attacker.ReturnToStart());
            }

            bool IsTeamDead(bool players)
            {
                return _allUnits
                    .Where(u => u.IsPlayer == players)
                    .All(u => u.IsDead);
            }
            IEnumerator HandleTurn(BattleUnit unit)
            {
                SetTurnText(unit);
                if (unit.IsBroken)
                {
                    unit.OnBrokenTurn();
                    yield return new WaitForSeconds(0.5f);
                    yield break;
                }
                if (!unit.IsPlayer)
                {
                    unit.StartHighlight();
                }
                if (unit.IsPlayer)
                {
                    yield return StartCoroutine(unit.StepOut());
                    while (true)
                    {
                        Debug.Log($"(GRACZ) Tura: {unit.data.characterName}. Wybierz akcję...");
                        waitingForActionChoice = true;
                        ShowActionButtons(true);
                        ShowTargetButtons(false);
                        yield return new WaitUntil(() => waitingForActionChoice == false);
                        playerCanceledTargetSelection = false;

                        if (_highlightedUnit != null)
                        {
                            _highlightedUnit.StopHighlight();
                            _highlightedUnit = null;
                        }
                        switch (chosenPlayerAction)
                        {
                            case BattleActionType.Attack:
                                waitingForTargetChoice = true;
                                ShowActionButtons(false);
                                ShowTargetButtons(true);
                                UpdateEnemyTargetButtons();

                                Debug.Log("Wybierz wroga (1–4) albo cofnij.");
                                yield return new WaitUntil(() => waitingForTargetChoice == false);

                                if (playerCanceledTargetSelection)
                                {
                                    ShowTargetButtons(false);
                                    continue;
                                }
                                BattleUnit enemyTarget = GetEnemyByIndex(chosenTargetIndex);
                                if (enemyTarget != null)
                                    yield return StartCoroutine(PerformAttack(unit, enemyTarget));
                                else
                                    Debug.Log("Wybrany wróg nie istnieje lub jest martwy.");
                                break;

                            case BattleActionType.Heal:
                                waitingForTargetChoice = true;
                                ShowActionButtons(false);
                                ShowTargetButtons(true);
                                UpdateAllyTargetButtons();

                                Debug.Log("Wybierz sojusznika do leczenia (1–4) albo cofnij.");
                                yield return new WaitUntil(() => waitingForTargetChoice == false);

                                if (playerCanceledTargetSelection)
                                {
                                    ShowTargetButtons(false);
                                    continue;
                                }
                                BattleUnit allyTarget = GetAllyByIndex(chosenTargetIndex);
                                if (allyTarget != null)
                                {
                                    yield return StartCoroutine(PerformHeal(unit, allyTarget));
                                }
                                break;

                            case BattleActionType.Defend:
                                unit.Defend();
                                Debug.Log($"{unit.data.characterName} wybiera Defend.");
                                yield return new WaitForSeconds(0.5f);
                                break;

                            case BattleActionType.Skip:
                                Debug.Log($"{unit.data.characterName} pomija turę.");
                                yield return new WaitForSeconds(0.5f);
                                break;
                        }
                        ShowActionButtons(false);
                        ShowTargetButtons(false);

                        unit.StopHighlight();
                        yield return StartCoroutine(unit.ReturnToStart());
                        yield break;
                    }
                }
                Debug.Log($"(WRÓG) Tura: {unit.data.characterName}. Czekam {enemyDecisionDelay}s...");
                ShowActionButtons(false);
                ShowTargetButtons(false);
                yield return new WaitForSeconds(enemyDecisionDelay);

                bool canHeal = unit.data.healPower > 0;
                var woundedAllies = _allUnits
                    .Where(u => u.IsPlayer == unit.IsPlayer && !u.IsDead && u.CurrentHP < u.data.maxHP)
                    .ToList();

                BattleActionType enemyAction = BattleActionType.Attack;
                if (canHeal && woundedAllies.Count > 0)
                {
                    // np. 60% szansy, że wybierze Heal zamiast Attack
                    float healChance = 0.6f;
                    if (Random.value < healChance)
                        enemyAction = BattleActionType.Heal;
                }

                switch (enemyAction)
                {
                    case BattleActionType.Attack:
                        var possibleTargets = _allUnits
                            .Where(u => u.IsPlayer && !u.IsDead)
                            .ToList();

                        BattleUnit enemyTarget = possibleTargets.Count > 0
                            ? possibleTargets[Random.Range(0, possibleTargets.Count)]
                            : null;

                        if (enemyTarget != null)
                            yield return StartCoroutine(PerformAttack(unit, enemyTarget));
                        break;

                    case BattleActionType.Heal:
                        var allies = _allUnits
                                .Where(u => u.IsPlayer == unit.IsPlayer && !u.IsDead && u.CurrentHP < u.data.maxHP)
                                .OrderBy(u => (float)u.CurrentHP / u.data.maxHP)
                                .ToList();

                        if (allies.Count > 0)
                        {
                            var healTarget = allies[0];
                            Debug.Log($"(WRÓG) {unit.data.characterName} leczy {healTarget.data.characterName}");
                            yield return StartCoroutine(PerformHeal(unit, healTarget));
                        }
                        else
                        {
                            goto case BattleActionType.Attack;
                        }
                        break;

                    case BattleActionType.Skip:
                        Debug.Log($"{unit.data.characterName} pomija turę.");
                        yield return new WaitForSeconds(0.5f);
                        break;
                }
                if (!unit.IsPlayer)
                {
                    unit.StopHighlight();
                }
            }
        }
    }
    
    public void OnTargetBackButton()
    {
        if (!waitingForTargetChoice)
            return;

        Debug.Log("Gracz cofa wybór celu, wracam do menu akcji.");
        foreach (var u in _allUnits)
            u.StopHighlight();

        if (_highlightedUnit != null)
        {
            _highlightedUnit.StopHighlight();
            _highlightedUnit = null;
        }
        playerCanceledTargetSelection = true;
        waitingForTargetChoice = false;
    }
    IEnumerator PerformHeal(BattleUnit healer, BattleUnit target)
    {
        if (target == null || target.IsDead) yield break;
        Debug.Log($"{healer.data.characterName} leczy {target.data.characterName}");

        DoHeal(healer, target);
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(healer.ReturnToStart());
    }
    public void DoHeal(BattleUnit healer)
    {
        var allies = _allUnits
            .Where(u => u.IsPlayer == healer.IsPlayer && !u.IsDead)
            .ToList();

        if (allies.Count == 0) return;

        var target = allies[Random.Range(0, allies.Count)];
        target.Heal(healer.data.healPower);
        Debug.Log($"{healer.data.characterName} leczy {target.data.characterName}");
    }
    void DoHeal(BattleUnit healer, BattleUnit target)
    {
        if (target == null || target.IsDead) return;

        target.Heal(healer.data.healPower);
        Debug.Log($"{healer.data.characterName} leczy {target.data.characterName}");
    }
    public void OnAttackButton()
    {
        if (!waitingForActionChoice) return;
        chosenPlayerAction = BattleActionType.Attack;
        waitingForActionChoice = false;
    }
    public void OnHealButton()
    {
        if (!waitingForActionChoice) return;
        chosenPlayerAction = BattleActionType.Heal;
        waitingForActionChoice = false;
    }
    public void OnDefendButton()
    {
        if (!waitingForActionChoice) return;
        chosenPlayerAction = BattleActionType.Defend;
        waitingForActionChoice = false;
    }
    public void OnSkipButton()
    {
        if (!waitingForActionChoice) return;
        chosenPlayerAction = BattleActionType.Skip;
        waitingForActionChoice = false;
    }
    void SelectTarget(int index)
    {
        if (!waitingForTargetChoice) return;
        chosenTargetIndex = index;

        if (chosenPlayerAction == BattleActionType.Attack)
        {
            BattleUnit enemy = GetEnemyByIndex(index);
            if (enemy != null)
            {
                StartCoroutine(enemy.HitFlash());
            }
        }
        else if (chosenPlayerAction == BattleActionType.Heal)
        {
            BattleUnit ally = GetAllyByIndex(index);
            if (ally != null)
            {
                StartCoroutine(ally.HitFlash());
            }
        }

        foreach (var u in _allUnits)
            u.StopHighlight();

        if (_highlightedUnit != null)
        {
            _highlightedUnit.StopHighlight();
            _highlightedUnit = null;
        }

        waitingForTargetChoice = false;
    }
    public void OnTargetButton1() => SelectTarget(0);
    public void OnTargetButton2() => SelectTarget(1);
    public void OnTargetButton3() => SelectTarget(2);
    public void OnTargetButton4() => SelectTarget(3); 
    public void PreviewTarget(int index)
    {
        if (!waitingForTargetChoice) return;

        if (chosenPlayerAction == BattleActionType.Attack)
        {
            BattleUnit enemy = GetEnemyByIndex(index);
            if (enemy != null)
            {
                StartCoroutine(enemy.HitFlash());
            }
        }
        else if (chosenPlayerAction == BattleActionType.Heal)
        {
            BattleUnit ally = GetAllyByIndex(index);
            if (ally != null)
            {
                StartCoroutine(ally.HitFlash());
            }
        }
    }
    public void PreviewHighlight(int index, bool state)
    {
        if (!waitingForTargetChoice)
        {
            if (_highlightedUnit != null)
            {
                _highlightedUnit.StopHighlight();
                _highlightedUnit = null;
            }
            return;
        }

        if (!state)
        {
            if (_highlightedUnit != null)
            {
                _highlightedUnit.StopHighlight();
                _highlightedUnit = null;
            }
            return;
        }
        BattleUnit unit = null;

        if (chosenPlayerAction == BattleActionType.Attack)
            unit = GetEnemyByIndex(index);
        else if (chosenPlayerAction == BattleActionType.Heal)
            unit = GetAllyByIndex(index);

        if (unit == _highlightedUnit)
            return;

        if (_highlightedUnit != null)
            _highlightedUnit.StopHighlight();

        _highlightedUnit = unit;

        if (_highlightedUnit != null)
            _highlightedUnit.StartHighlight();
    }
    void SetTurnText(BattleUnit unit)
    {
        if (turnText == null) return;
        string side = unit.IsPlayer ? "Tura gracza" : "Tura wroga";
        turnText.text = $"{side}: {unit.data.characterName}";
    }
    void ShowActionButtons(bool show)
    {
        if (actionButtonsPanel != null)
            actionButtonsPanel.SetActive(show);
    }
    void ShowTargetButtons(bool show)
    {
        if (targetButtonsPanel != null)
            targetButtonsPanel.SetActive(show);
    }
    BattleUnit GetEnemyByIndex(int index)
    {
        if (index < 0 || index >= _enemyUnits.Count) return null;

        var enemy = _enemyUnits[index];
        if (enemy == null || enemy.IsDead || !enemy.gameObject.activeSelf) return null;

        return enemy;
    }
    BattleUnit GetAllyByIndex(int index)
    {
        if (index < 0 || index >= _playerUnits.Count) return null;

        var ally = _playerUnits[index];
        if (ally == null || ally.IsDead || !ally.gameObject.activeSelf) return null;

        return ally;
    }
}