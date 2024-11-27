using UnityEngine;

public class CannonController : MonoBehaviour
{
    /// <summary>
    /// Calcula la direcci�n hacia la cual el ca��n debe disparar para interceptar un objetivo en movimiento.
    /// </summary>
    /// <param name="cannonPosition">Posici�n del ca��n.</param>
    /// <param name="targetPosition">Posici�n actual del objetivo.</param>
    /// <param name="targetVelocity">Velocidad del objetivo.</param>
    /// <param name="projectileSpeed">Velocidad de la bala disparada por el ca��n.</param>
    /// <returns>Direcci�n en la que debe disparar el ca��n para interceptar el objetivo.</returns>
    public static Vector3 CalculateInterceptDirection(Vector3 cannonPosition, Vector3 targetPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 toTarget = targetPosition - cannonPosition;
        float targetSpeedSquared = targetVelocity.sqrMagnitude;

        // Si el objetivo no se mueve o la bala es infinitamente r�pida
        if (targetSpeedSquared < 0.001f || projectileSpeed <= 0.0f)
        {
            return toTarget.normalized; // Dispara directamente hacia el objetivo.
        }

        float a = targetSpeedSquared - projectileSpeed * projectileSpeed;
        float b = 2 * Vector3.Dot(toTarget, targetVelocity);
        float c = toTarget.sqrMagnitude;

        // Resuelve la ecuaci�n cuadr�tica: at^2 + bt + c = 0
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No hay soluci�n, dispara directamente hacia el objetivo.
            return toTarget.normalized;
        }

        // Calcula el tiempo m�s corto para interceptar
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

        // Calcula el punto de intercepci�n
        Vector3 interceptPoint = targetPosition + targetVelocity * interceptTime;

        // Devuelve la direcci�n normalizada hacia el punto de intercepci�n
        return (interceptPoint - cannonPosition).normalized;
    }
}