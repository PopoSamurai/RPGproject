using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticalTurnManager : MonoBehaviour
{
    [Header("Kolejnoœæ jednostek")]
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
        if (units.Count == 0)
            return;

        if (ActiveUnit != null)
            ActiveUnit.EndTurn();

        // Usuñ jednostki zniszczone lub usuniête z gry.
        units.RemoveAll(unit => unit == null || !unit.IsAlive);

        if (units.Count == 0)
        {
            ActiveUnit = null;
            UpdateUI();
            Debug.Log("Brak pozosta³ych jednostek.");
            return;
        }

        currentUnitIndex++;

        if (currentUnitIndex >= units.Count)
        {
            currentUnitIndex = 0;
            turnNumber++;
        }

        ActivateCurrentUnit();
    }

    public void SelectUnit(int index)
    {
        if (index < 0 || index >= units.Count)
            return;

        if (ActiveUnit != null)
            ActiveUnit.EndTurn();

        currentUnitIndex = index;
        ActivateCurrentUnit();
    }

    private void ActivateCurrentUnit()
    {
        if (units.Count == 0)
            return;

        ActiveUnit = units[currentUnitIndex];

        if (ActiveUnit == null || !ActiveUnit.IsAlive)
        {
            NextTurn();
            return;
        }

        ActiveUnit.BeginTurn();

        if (gridMovement != null)
        {
            // Aktualna jednostka staje siê jednostk¹ sterowan¹
            // przez istniej¹cy system ruchu.
            gridMovement.player = ActiveUnit.transform;

            gridMovement.movementRange = ActiveUnit.movementRange;
            gridMovement.moveSpeed = ActiveUnit.moveSpeed;
            gridMovement.carryStrength = ActiveUnit.strength;
            gridMovement.attackDamage = ActiveUnit.attackDamage;
            gridMovement.isArcher = ActiveUnit.isArcher;
        }

        if (tacticalCamera != null)
            tacticalCamera.SetTarget(ActiveUnit.transform);

        UpdateUI();

        Debug.Log(
            $"Tura {turnNumber}: aktywna jednostka - {ActiveUnit.name}"
        );
    }

    private void UpdateUI()
    {
        if (activeUnitText != null)
        {
            activeUnitText.text = ActiveUnit != null
                ? $"Jednostka: {ActiveUnit.name}"
                : "Brak aktywnej jednostki";
        }

        if (turnNumberText != null)
            turnNumberText.text = $"Tura: {turnNumber}";

        if (nextTurnButton != null)
            nextTurnButton.interactable = units.Count > 0;
    }
}