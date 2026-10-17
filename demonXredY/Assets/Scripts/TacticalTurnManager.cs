using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticalTurnManager : MonoBehaviour
{
    [Header("Kolejność jednostek")]
    public List<TacticalUnit> units = new List<TacticalUnit>();

    [Header("Integracja")]
    public TacticalGridMovement gridMovement;
    public TacticalCameraController tacticalCamera;

    [Header("UI - opcjonalne")]
    public Text activeUnitText;
    public Text turnNumberText;
    public Button nextTurnButton;

    [Header("Stan")]
    [SerializeField] private int currentUnitIndex = 0;
    [SerializeField] private int turnNumber = 1;

    public TacticalUnit ActiveUnit { get; private set; }
    public int TurnNumber => turnNumber;

    [Header("Battle result UI (optional)")]
    public GameObject resultPanel;
    public Text resultText;
    public string victoryMessage = "ZWYCIĘSTWO";
    public string defeatMessage = "PORAŻKA";
    public bool BattleFinished { get; private set; }
    private bool enemyTurnRunning;
    private bool turnsStarted;


    private void Start()
    {
        if (nextTurnButton != null)
            nextTurnButton.onClick.AddListener(NextTurn);

        if (resultPanel != null) resultPanel.SetActive(false);
        StartTurns();
    }

    private void OnDestroy()
    {
        if (nextTurnButton != null)
            nextTurnButton.onClick.RemoveListener(NextTurn);
    }

    public void StartTurns()
    {
        turnsStarted = true;
        BattleFinished = false;
        ActiveUnit = null;
        currentUnitIndex = 0;
        turnNumber = 1;
        units.RemoveAll(unit => unit == null);

        if (units.Count == 0)
        {
            UpdateUI();
            Debug.LogWarning(
                "TacticalTurnManager: dodaj jednostki do listy Units."
            );
            return;
        }

        if (CheckBattleResult()) return;
        currentUnitIndex = Mathf.Clamp(
            currentUnitIndex, 0, units.Count - 1
        );

        ActivateCurrentUnit();
    }

    public void NextTurn()
    {
        if (BattleFinished || enemyTurnRunning || ActiveUnit == null || !ActiveUnit.isPlayerControlled || (gridMovement != null && gridMovement.IsBusy)) return;
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        if (BattleFinished || CheckBattleResult()) return;
        if (ActiveUnit != null) ActiveUnit.EndTurn();
        if (units.Count == 0) return;
        int checkedUnits = 0;
        do
        {
            currentUnitIndex++;
            if (currentUnitIndex >= units.Count) { currentUnitIndex = 0; turnNumber++; }
            checkedUnits++;
        }
        while (checkedUnits < units.Count && (units[currentUnitIndex] == null || !units[currentUnitIndex].IsAlive));
        if (checkedUnits >= units.Count && (units[currentUnitIndex] == null || !units[currentUnitIndex].IsAlive))
        {
            ActiveUnit = null;
            UpdateUI();
            return;
        }
        if (!CheckBattleResult()) ActivateCurrentUnit();
    }

    // Zaznaczenie jednostki nie zmienia kolejności tur.
    public void SelectUnit(int index) { }

    private void ActivateCurrentUnit()
    {
        if (units.Count == 0 || BattleFinished || CheckBattleResult()) return;
        ActiveUnit = units[currentUnitIndex];
        if (ActiveUnit == null || !ActiveUnit.IsAlive) { AdvanceTurn(); return; }
        ActiveUnit.BeginTurn();
        if (gridMovement != null) gridMovement.SetActiveUnit(ActiveUnit);
        if (tacticalCamera != null) tacticalCamera.SetTarget(ActiveUnit.transform);
        UpdateUI();
        Debug.Log($"Tura {turnNumber}: {GetUnitName(ActiveUnit)}");
        if (!ActiveUnit.isPlayerControlled) StartCoroutine(EnemyTurn(ActiveUnit));
    }

    private IEnumerator EnemyTurn(TacticalUnit enemy)
    {
        if (BattleFinished) yield break;
        enemyTurnRunning = true;
        if (gridMovement != null) yield return StartCoroutine(gridMovement.RunEnemyTurn(enemy));
        yield return new WaitForSeconds(0.35f);
        enemyTurnRunning = false;
        if (!CheckBattleResult()) AdvanceTurn();
    }

    private void UpdateUI()
    {
        if (activeUnitText != null)
        {
            activeUnitText.text = ActiveUnit != null
                ? $"Jednostka: {GetUnitName(ActiveUnit)}"
                : "Brak aktywnej jednostki";
        }

        if (turnNumberText != null)
            turnNumberText.text = $"Tura: {turnNumber}";

        if (nextTurnButton != null)
            nextTurnButton.interactable = ActiveUnit != null && ActiveUnit.isPlayerControlled && !enemyTurnRunning && !BattleFinished;
    }
    private string GetUnitName(TacticalUnit unit)
    {
        return unit != null && unit.unitData != null && !string.IsNullOrEmpty(unit.unitData.displayName)
            ? unit.unitData.displayName : (unit != null ? unit.name : "-");
    }

    private void Update()
    {
        if (turnsStarted && !BattleFinished) CheckBattleResult();
    }

    private bool CheckBattleResult()
    {
        if (BattleFinished) return true;
        int players = 0, enemies = 0;
        foreach (TacticalUnit unit in units)
        {
            if (unit == null || !unit.IsAlive || !unit.gameObject.activeInHierarchy) continue;
            if (unit.isPlayerControlled) players++; else enemies++;
        }
        if (players > 0 && enemies > 0) return false;
        BattleFinished = true;
        if (ActiveUnit != null) ActiveUnit.EndTurn();
        ActiveUnit = null;
        enemyTurnRunning = false;
        if (gridMovement != null) gridMovement.SetActiveUnit(null);
        string message = players == 0 ? defeatMessage : victoryMessage;
        if (resultText != null) resultText.text = message;
        if (resultPanel != null) resultPanel.SetActive(true);
        Debug.Log("Battle ended: " + message);
        UpdateUI();
        return true;
    }
}