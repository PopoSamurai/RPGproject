using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to an always-active GameObject (e.g. TacticalTurnManager), NOT to actionPanel.
/// Assign actionPanel and the buttons in the Inspector.
/// </summary>
public class TacticalActionPanel : MonoBehaviour
{
    [Header("References")]
    public TacticalTurnManager turnManager;
    public TacticalGridMovement gridMovement;

    [Header("UI")]
    public GameObject actionPanel;
    public Button moveButton;
    public Button jumpButton;
    public Button attackButton;
    public Button throwButton;
    public Button endTurnButton;

    private void LateUpdate()
    {
        TacticalUnit unit = turnManager != null ? turnManager.ActiveUnit : null;
        bool playerTurn = unit != null && unit.IsAlive && unit.IsActiveUnit && unit.isPlayerControlled;

        if (actionPanel != null && actionPanel.activeSelf != playerTurn)
            actionPanel.SetActive(playerTurn);

        if (!playerTurn) return;

        bool busy = gridMovement != null && gridMovement.IsBusy;
        bool canMove = !busy && unit.RemainingMovement > 0;
        bool canAct = !busy && !unit.hasActed;

        if (moveButton != null) moveButton.interactable = canMove;
        if (jumpButton != null) jumpButton.interactable = canMove;
        if (attackButton != null) attackButton.interactable = canAct;
        if (throwButton != null) throwButton.interactable = canAct;
        if (endTurnButton != null) endTurnButton.interactable = !busy;
    }
}
