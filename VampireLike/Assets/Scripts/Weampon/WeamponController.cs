using UnityEngine;

public class WeamponController : MonoBehaviour
{
    [Header("Weampon Stats")]
    public WeamponScriptableObj weamponData;
    float currentCooldown;
    protected Movement player;
    protected virtual void Start()
    {
        player = FindObjectOfType<Movement>();
        currentCooldown = weamponData.CooldownDuration;
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        currentCooldown = weamponData.CooldownDuration;
    }
}