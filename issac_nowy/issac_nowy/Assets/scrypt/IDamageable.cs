using UnityEngine;

// IDamageable.cs
public interface IDamageable
{
    void TakeDamage(float amount);
    bool IsAlive { get; }
}
