using UnityEngine;
public class TacticalUnit : MonoBehaviour
{
    [Header("Dane jednostki (wymagane)")]
    public TacticalUnitData unitData;
    public TacticalItemData equippedWeapon;
    public TacticalItemData equippedArmor;

    public TacticalSkillData[] AvailableSkills => unitData != null ? unitData.skills : null;
    public TacticalElement Element => unitData != null ? unitData.element : TacticalElement.Fire;
    public TacticalSubclassData Subclass => unitData != null ? unitData.subclass : null;
    public TacticalClassData UnitClass => unitData != null ? unitData.unitClass : null;
    public const int MaxLevel = 50;
    [Header("Progression (runtime)")]
    [SerializeField, Range(1, 50)] private int level = 1;
    [SerializeField] private int experience;
    [SerializeField] private int currentMana;
    public int Level => level;
    public int Experience => experience;
    public int CurrentMana => currentMana;
    public int MaxMana => calculatedStats.mp;
    public TacticalStats Stats => calculatedStats;
    public int Defense => calculatedStats.def;
    public int Speed => calculatedStats.spd;
    public int Intelligence => calculatedStats.intelligence;
    public int Accuracy => calculatedStats.accuracy;
    public int Evasion => calculatedStats.evasion;
    public int JumpHeight => calculatedStats.jump;
    public int JumpDistance => calculatedStats.jumpDistance;
    public int FireResistance => calculatedStats.fireRes;
    public int IceResistance => calculatedStats.iceRes;
    public int LightningResistance => calculatedStats.lightningRes;
    public int LightResistance => calculatedStats.lightRes;
    public int DarkResistance => calculatedStats.darkRes;
    public bool CanEquip(TacticalItemData item)
    {
        if (item == null) return true;
        TacticalSubclassData sub = unitData != null ? unitData.subclass : null;
        if (sub == null) return true;
        if (item.type == TacticalItemType.Armor && item.armorClass > sub.maximumArmor) return false;
        if (item.isShield && !sub.canUseShields) return false;
        return true;
    }
    public void RefreshStats()
    {
        int previousMaxHp = maxHealth;
        int previousMaxMp = MaxMana;
        LoadUnitData();
        currentHealth = Mathf.Clamp(currentHealth + Mathf.Max(0, maxHealth - previousMaxHp), 0, maxHealth);
        currentMana = Mathf.Clamp(currentMana + Mathf.Max(0, MaxMana - previousMaxMp), 0, MaxMana);
        remainingMovement = Mathf.Min(remainingMovement, movementRange);
    }
    private TacticalStats calculatedStats;

    public static int ExperienceToNextLevel(int currentLevel)
    {
        return currentLevel >= MaxLevel ? 0 : Mathf.RoundToInt(100f * Mathf.Pow(currentLevel, 1.5f));
    }

    public void GainExperience(int amount)
    {
        if (amount <= 0 || level >= MaxLevel) return;
        experience += amount;
        while (level < MaxLevel && experience >= ExperienceToNextLevel(level))
        {
            experience -= ExperienceToNextLevel(level);
            SetLevel(level + 1);
        }
        if (level >= MaxLevel) experience = 0;
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Clamp(newLevel, 1, MaxLevel);
        RefreshStats();
    }


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
        calculatedStats = unitData.GetStatsAtLevel(level);
        moveSpeed = unitData.moveSpeed;
        if (unitData.basicAttack != null)
        {
            attackDamage = unitData.basicAttack.damage;
            attackRange = Mathf.Max(1, unitData.basicAttack.range);
            isArcher = unitData.basicAttack.ranged;
        }
        ApplyEquipment(equippedWeapon);
        ApplyEquipment(equippedArmor);
        // Wrodzona odpornosc: jednostka ma +25 punktow procentowych
        // do odpornosci swojego zywiolu. Bonus jest runtime-only:
        // nie modyfikuje assetu ScriptableObject i nie kumuluje sie po RefreshStats().
        switch (Element)
        {
            case TacticalElement.Fire: calculatedStats.fireRes += 25; break;
            case TacticalElement.Ice: calculatedStats.iceRes += 25; break;
            case TacticalElement.Lightning: calculatedStats.lightningRes += 25; break;
            case TacticalElement.Light: calculatedStats.lightRes += 25; break;
            case TacticalElement.Dark: calculatedStats.darkRes += 25; break;
        }
        maxHealth = calculatedStats.hp;
        strength = calculatedStats.str;
        movementRange = calculatedStats.movement;
        weight = calculatedStats.weight;

    }

    private void ApplyEquipment(TacticalItemData item)
    {
        if (item == null) return;
        if (!CanEquip(item))
        {
            Debug.LogWarning($"{name}: item {item.name} is not allowed for this subclass.", this);
            return;
        }
        attackDamage += item.bonusAttack;
        calculatedStats.hp += item.bonusHealth;
        calculatedStats.str += item.bonusStrength;
        calculatedStats.movement += item.bonusMovement;
        calculatedStats.def += item.bonusDefense;
        calculatedStats.mp += item.bonusMana;
        calculatedStats.weight += Mathf.Max(0, item.itemWeight);
        calculatedStats.jump = Mathf.Max(0, calculatedStats.jump + item.bonusJump);
        calculatedStats.jumpDistance = Mathf.Max(0, calculatedStats.jumpDistance + item.bonusJumpDistance);
    }

    // Called by the battle spawner after Instantiate (Awake may have run already).
    public void Initialize(TacticalUnitData data, bool playerTeam)
    {
        unitData = data;
        isPlayerControlled = playerTeam;
        if (unitData == null)
        {
            Debug.LogError($"{name}: brak Unit Data w TacticalBattleSetup!", this);
            enabled = false;
            return;
        }

        enabled = true;
        level = Mathf.Clamp(unitData.startingLevel, 1, MaxLevel);
        experience = 0;
        LoadUnitData();
        currentHealth = maxHealth;
        currentMana = MaxMana;
        remainingMovement = 0;
        hasMoved = false;
        hasActed = false;
        hasJumpedThisTurn = false;
        isActiveUnit = false;
    }

    [Header("Druzyna / sterowanie")]
    public bool isPlayerControlled = true;

    // Statystyki sa wyliczane z UnitData i wyposazenia, a nie ustawiane na jednostce.
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
    [Header("Jump limit")]
    [SerializeField] private bool hasJumpedThisTurn;
    public bool HasJumpedThisTurn => hasJumpedThisTurn;
    public bool CanJumpThisTurn => IsAlive && isActiveUnit && !hasJumpedThisTurn && remainingMovement >= 2;

    public bool TryUseJump()
    {
        if (!CanJumpThisTurn) return false;
        hasJumpedThisTurn = true;
        SpendMovement(2);
        return true;
    }

    public bool hasActed = false;

    [SerializeField] private bool isActiveUnit = false;

    public bool IsActiveUnit => isActiveUnit;

    private void Awake()
    {
        // Instantiate calls Awake before TacticalBattleSetup.Initialize.
        // An empty Unit Data field on the shared spawn prefab is expected.
        if (unitData == null) return;

        level = Mathf.Clamp(unitData.startingLevel, 1, MaxLevel);
        LoadUnitData();
        currentMana = MaxMana;
        currentHealth = maxHealth;
    }

    public void BeginTurn()
    {
        if (!IsAlive)
            return;

        isActiveUnit = true;
        hasMoved = false;
        hasActed = false;
        hasJumpedThisTurn = false;
        remainingMovement = movementRange;

        Debug.Log($"{name}: rozpoczêcie tury.");
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
            $"{name}: otrzymano {damage} obra¿eñ. " +
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
        Debug.Log($"{name} zosta³a pokonana.");
        // Na razie usuwamy jednostkê z gry.
        // PóŸniej mo¿na tu dodaæ animacjê œmierci i system tur.
        gameObject.SetActive(false);
    }
}