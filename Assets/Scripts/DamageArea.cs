using SpaceGame;
using System.Linq;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField, Tooltip("Damage dealt per tick")]
    private float damagePerTick = 10f;

    [SerializeField, Tooltip("Damage dealt per tick")]
    private float tickInterval = 1f;

    [SerializeField, Tooltip("Appply damage to GameObjects with specified tags")]
    private string[] tags = { };

    private float nextTickTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (!IsValidDamageTarget(other.gameObject.tag)) return;

        // Check if the time has passed for the next tick
        if (Time.time < nextTickTime) return;
        nextTickTime = Time.time + tickInterval;

        // Attempt to damage the target
        DamageTarget(other);
    }

    /// <summary>
    /// Filter by tags if not empty.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private bool IsValidDamageTarget(string tag)
    {
        // No filters applied, is valid.
        if (tags.Length <= 0) return true;
        return tags.Contains(tag);
    }

    private void DamageTarget(Collider other)
    {
        other.TryGetComponent(out Health target);
        if (target == null) return;
        target.ChangeHealth(-damagePerTick);
    }
}
