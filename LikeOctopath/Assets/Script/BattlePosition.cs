using UnityEngine;

public class BattlePosition : MonoBehaviour
{
    public bool isPlayerSlot;
    public Transform stepOutPoint;
    [HideInInspector] public BattleUnit currentUnit;
}