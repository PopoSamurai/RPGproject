using UnityEngine;
public class TacticalUnit : MonoBehaviour
{
    [Header("Kontrola")]
    public bool isPlayerControlled = true;
    public bool isArcher = false;

    [Header("Statystyki")]
    [Min(1)] public int maxHealth = 10;
    [Min(0)] public int strength = 2;
    [Min(1)] public int attackDamage = 2;
    [Min(1)] public int movementRange = 5;
    [Min(0.1f)] public float moveSpeed = 4f;

    [Min(1)]
    [Tooltip("Waga obiektu. Gracz podniesie obiekt, jeœli jego Carry Strength jest >= tej wartoœci.")]
    public int weight = 1;

    [Header("Atak dystansowy")]
    [Min(1)] public int attackRange = 4;
    [Header("Stan")]
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;

    [Header("Stan tury")]
    public bool hasMoved = false;
    public bool hasActed = false;

    [SerializeField] private bool isActiveUnit = false;

    public bool IsActiveUnit => isActiveUnit;

    private void Awake()
    {

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