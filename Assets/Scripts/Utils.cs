using UnityEngine;

namespace SpaceGame
{
    public static class Utils
    {
        public static Vector3 PredictV3Pos(Vector3 muzzlePos, float bulletVelocity, Vector3 targetPos, Vector3 targetVelocity, bool useGravity = false)
        {
            Vector3 totarget = targetPos - muzzlePos;

            float a = Vector3.Dot(targetVelocity, targetVelocity) - (bulletVelocity * bulletVelocity);

            if (a == 0F) { Debug.Log("wasd"); }
            if (bulletVelocity == 0F) { Debug.Log("bbb"); }

            float b = 2 * Vector3.Dot(targetVelocity, totarget);
            float c = Vector3.Dot(totarget, totarget);

            float p = -b / (2 * a);
            float q = Mathf.Sqrt((b * b) - 4 * a * c) / (2 * a);

            float t1 = p - q;
            float t2 = p + q;
            float t;

            if (t1 > t2 && t2 > 0)
            {
                t = t2;
            }
            else
            {
                t = t1;
            }

            Vector3 aimSpot = targetPos + targetVelocity * t;
            Vector3 bulletPath = aimSpot - muzzlePos;

            // If no drag
            if (useGravity)
            {
                float timeToImpact = bulletPath.magnitude / bulletVelocity; // Speed must be in units per second
                Vector3 gravityDrop = 0.5f * Physics.gravity * timeToImpact * timeToImpact; // 1/2gt^2
                return aimSpot - gravityDrop;
            }

            return aimSpot;
        }

        public static Vector3 InterceptTarget(GameObject shooter, GameObject target, float shotSpeed)
        {
            // Positions
            Vector3 shooterPosition = shooter.transform.position;
            Vector3 targetPosition = target.transform.position;

            // Velocities
            Vector3 shooterVelocity = shooter.TryGetComponent(out Rigidbody shooterRigidbody) ? shooterRigidbody.linearVelocity : Vector3.zero;
            Vector3 targetVelocity = target.TryGetComponent(out Rigidbody targetRigidbody) ? targetRigidbody.linearVelocity : Vector3.zero;

            // Calculate intercept
            Vector3 interceptPoint = FirstOrderIntercept(shooterPosition, shooterVelocity, shotSpeed, targetPosition, targetVelocity);
            return interceptPoint;
        }

        /// <summary>
        /// First-order intercept using absolute target position
        /// </summary>
        /// <param name="shooterPosition"></param>
        /// <param name="shooterVelocity"></param>
        /// <param name="shotSpeed"></param>
        /// <param name="targetPosition"></param>
        /// <param name="targetVelocity"></param>
        /// <returns></returns>
        // https://web.archive.org/web/20210115145043/http://wiki.unity3d.com/index.php?title=Calculating_Lead_For_Projectiles
        public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
        {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);
            return targetPosition + t * (targetRelativeVelocity);
        }

        /// <summary>
        /// First-order intercept using relative target position
        /// </summary>
        /// <param name="shotSpeed"></param>
        /// <param name="targetRelativePosition"></param>
        /// <param name="targetRelativeVelocity"></param>
        /// <returns></returns>
        // https://web.archive.org/web/20210115145043/http://wiki.unity3d.com/index.php?title=Calculating_Lead_For_Projectiles
        public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
        {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f)
            {
                return 0f;
            }

            float a = velocitySquared - shotSpeed * shotSpeed;

            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
                return Mathf.Max(t, 0f); //don't shoot back in time
            }

            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            if (determinant > 0f)
            {
                //determinant > 0; two intercept paths (most common)
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
                float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
                if (t1 > 0f)
                {
                    if (t2 > 0f)
                    {
                        return Mathf.Min(t1, t2); //both are positive
                    }
                    else
                    {
                        return t1; //only t1 is positive
                    }
                }
                else
                {
                    return Mathf.Max(t2, 0f); //don't shoot back in time
                }
            }
            else if (determinant < 0f) //determinant < 0; no intercept path
            {
                return 0f;
            }
            else //determinant = 0; one intercept path, pretty much never happens
            {
                return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
            }
        }
    }
}
