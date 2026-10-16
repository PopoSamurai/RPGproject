using UnityEngine;
public class TacticalUnit : MonoBehaviour
{
    [Header("Dane jednostki (wymagane)")]
    public TacticalUnitData unitData;
    public TacticalItemData equippedWeapon;
    public TacticalItemData equippedArmor;
    public TacticalSkillData[] AvailableSkills => unitData != null ? unitData.skills : null;

    private void LoadUnitData()
    {
        if (unitData == null)
        {
            Debug.LogError($"{name}: brak Unit Data! Przypisz TacticalUnitData w Inspectorze.", this);
            enabled = false;
            return;
        }
        attackDamage = 0;
        attackRange = 1;
        isArcher = false;
        maxHealth = unitData.maxHealth;
        strength = unitData.strength;
        movementRange = unitData.movementRange;
        moveSpeed = unitData.moveSpeed;
        weight = unitData.weight;
        if (unitData.basicAttack != null)
        {
            attackDamage = unitData.basicAttack.damage;
            attackRange = unitData.basicAttack.range;
            isArcher = unitData.basicAttack.ranged;
        }
        if (equippedWeapon != null)
        {
            attackDamage += equippedWeapon.bonusAttack;
            strength += equippedWeapon.bonusStrength;
            movementRange += equippedWeapon.bonusMovement;
            maxHealth += equippedWeapon.bonusHealth;
        }
        if (equippedArmor != null)
        {
            attackDamage += equippedArmor.bonusAttack;
            strength += equippedArmor.bonusStrength;
            movementRange += equippedArmor.bonusMovement;
            maxHealth += equippedArmor.bonusHealth;
        }
    }

    [Header("Druzyna / sterowanie")]
    public bool isPlayerControlled = true;

    // Statystyki
    public bool isArcher { get; private set; }
    public int maxHealth { get; private set; }
    public int strength { get; private set; }
    public int attackDamage { get; private set; }
    public int movementRange { get; private set; }
    public float moveSpeed { get; private set; }
    public int weight { get; private set; }
    public int attackRange { get; private set; }

    [Header("Stan")]
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => unitData != null && currentHealth > 0;

    [Header("Stan tury")]
    public bool hasMoved = false;
    [SerializeField] private int remainingMovement;
    public int RemainingMovement => remainingMovement;
    public void SpendMovement(int cells)
    {
        remainingMovement = Mathf.Max(0, remainingMovement - Mathf.Max(0, cells));
        hasMoved = remainingMovement == 0;
    }
    public bool hasActed = false;

    [SerializeField] private bool isActiveUnit = false;

    public bool IsActiveUnit => isActiveUnit;

    private void Awake()
    {
        LoadUnitData();
        if (unitData == null) return;

        currentHealth = Mathf.Clamp(
            currentHealth <= 0 ? maxHealth : currentHealth,
            0,
            maxHealth
        );
    }
    public void BeginTurn()
    {
        if (!IsAlive)
            return;

        isActiveUnit = true;
        hasMoved = false;
        hasActed = false;
        remainingMovement = movementRange;

        Debug.Log($"{name}: rozpoczecie tury");
    }
    public void EndTurn()
    {
        isActiveUnit = false;

        Debug.Log($"{name}: koniec tury.");
    }
    public void ApplyDamage(int damage)
    {
        if (!IsAlive || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log(
            $"{name}: otrzymano {damage} obrazen " +
            $"HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0)
            Die();
    }
    public void Heal(int amount)
    {
        if (!IsAlive || amount <= 0)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
    private void Die()
    {
        Debug.Log($"pokonano {name}");
        gameObject.SetActive(false);
    }
}