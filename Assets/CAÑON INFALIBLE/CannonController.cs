using UnityEngine;

public class CannonController : MonoBehaviour
{
    /// <summary>
    /// Calcula la dirección hacia la cual el cañón debe disparar para interceptar un objetivo en movimiento.
    /// </summary>
    /// <param name="cannonPosition">Posición del cañón.</param>
    /// <param name="targetPosition">Posición actual del objetivo.</param>
    /// <param name="targetVelocity">Velocidad del objetivo.</param>
    /// <param name="projectileSpeed">Velocidad de la bala disparada por el cañón.</param>
    /// <returns>Dirección en la que debe disparar el cañón para interceptar el objetivo.</returns>
    public static Vector3 CalculateInterceptDirection(Vector3 cannonPosition, Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 toTarget = targetPosition - cannonPosition;
        float targetSpeedSquared = targetVelocity.sqrMagnitude;

        // Si el objetivo no se mueve o la bala es infinitamente rápida
        if (targetSpeedSquared < 0.001f || projectileSpeed <= 0.0f)
        {
            return toTarget.normalized; // Dispara directamente hacia el objetivo.
        }

        float a = targetSpeedSquared - projectileSpeed * projectileSpeed;
        float b = 2 * Vector3.Dot(toTarget, targetVelocity);
        float c = toTarget.sqrMagnitude;

        // Resuelve la ecuación cuadrática: at^2 + bt + c = 0
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No hay solución, dispara directamente hacia el objetivo.
            return toTarget.normalized;
        }

        // Calcula el tiempo más corto para interceptar
        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrtDiscriminant) / (2 * a);
        float t2 = (-b + sqrtDiscriminant) / (2 * a);
        float interceptTime = Mathf.Min(t1, t2);

        if (interceptTime < 0)
        {
            interceptTime = Mathf.Max(t1, t2); // Usa el tiempo positivo si existe.
        }

        if (interceptTime < 0)
        {
            return toTarget.normalized; // No es posible interceptar, dispara directamente.
        }

        // Calcula el punto de intercepción
        Vector3 interceptPoint = targetPosition + targetVelocity * interceptTime;

        // Devuelve la dirección normalizada hacia el punto de intercepción
        return (interceptPoint - cannonPosition).normalized;
    }
}