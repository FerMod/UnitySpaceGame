using SpaceGame;
using System.Linq;
using UnityEngine;

public class Instakill : MonoBehaviour
{
    [SerializeField, Tooltip("Insta kill GameObjects with specified tags")]
    private string[] tags = { };

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidDamageTarget(other.gameObject.tag)) return;
        KillTarget(other);
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

    private void KillTarget(Collider other)
    {
        other.TryGetComponent(out Health target);
        if (target == null) return;
        target.ChangeHealth(-target.MaxHealth);
    }
}
