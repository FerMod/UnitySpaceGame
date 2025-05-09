using UnityEngine;

namespace SpaceGame.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components, bool includeInactive = false) where T : Component
        {
            components = gameObject.GetComponentsInChildren<T>(includeInactive);
            return components != null && components.Length > 0;
        }
    }
}
