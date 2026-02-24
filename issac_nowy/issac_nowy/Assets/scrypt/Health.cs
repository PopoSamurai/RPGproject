// Health.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Zdrowie")]
    public float maxHealth = 100f;
    [SerializeField] public float currentHealth;

    [Header("Zdarzenia")]
    public UnityEvent onDeath;

    public bool IsAlive => currentHealth > 0f;

    private void Awake()
    {
        currentHealth = Mathf.Max(1f, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        currentHealth -= Mathf.Abs(amount);
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            onDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Abs(amount));
    }
    public static void Restart()
    {
        Time.timeScale = 1f; // na wszelki wypadek
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}
