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

    private bool enemyTurnRunning;

    private void Start()
    {
        if (nextTurnButton != null)
            nextTurnButton.onClick.AddListener(NextTurn);

        StartTurns();
    }

    private void OnDestroy()
    {
        if (nextTurnButton != null)
            nextTurnButton.onClick.RemoveListener(NextTurn);
    }

    public void StartTurns()
    {
        units.RemoveAll(unit => unit == null);

        if (units.Count == 0)
        {
            UpdateUI();
            Debug.LogWarning(
                "TacticalTurnManager: dodaj jednostki do listy Units."
            );
            return;
        }

        currentUnitIndex = Mathf.Clamp(
            currentUnitIndex, 0, units.Count - 1
        );

        ActivateCurrentUnit();
    }

    public void NextTurn()
    {
        if (enemyTurnRunning || (gridMovement != null && gridMovement.IsBusy)) return;
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
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
        ActivateCurrentUnit();
    }

    // Zaznaczenie jednostki nie zmienia kolejności tur.
    public void SelectUnit(int index) { }

    private void ActivateCurrentUnit()
    {
        if (units.Count == 0) return;
        ActiveUnit = units[currentUnitIndex];
        if (ActiveUnit == null || !ActiveUnit.IsAlive) { AdvanceTurn(); return; }
        ActiveUnit.BeginTurn();
        if (gridMovement != null) gridMovement.SetActiveUnit(ActiveUnit);
        if (tacticalCamera != null) tacticalCamera.SetTarget(ActiveUnit.transform);
        UpdateUI();
        Debug.Log($"Tura {turnNumber}: {ActiveUnit.name}");
        if (!ActiveUnit.isPlayerControlled) StartCoroutine(EnemyTurn(ActiveUnit));
    }

    private IEnumerator EnemyTurn(TacticalUnit enemy)
    {
        enemyTurnRunning = true;
        if (gridMovement != null) yield return StartCoroutine(gridMovement.RunEnemyTurn(enemy));
        yield return new WaitForSeconds(0.35f);
        enemyTurnRunning = false;
        AdvanceTurn();
    }

    private void UpdateUI()
    {
        if (activeUnitText != null)
        {
            if (ActiveUnit == null)
            {
                activeUnitText.text = "Brak aktywnej jednostki";
            }
            else if (ActiveUnit.unitData != null)
            {
                activeUnitText.text = ActiveUnit.unitData.displayName;
            }
            else
            {
                activeUnitText.text = ActiveUnit.name;
            }
        }

        if (turnNumberText != null)
            turnNumberText.text = $"Tura: {turnNumber}";

        if (nextTurnButton != null)
            nextTurnButton.interactable = ActiveUnit != null && ActiveUnit.isPlayerControlled && !enemyTurnRunning;
    }
}