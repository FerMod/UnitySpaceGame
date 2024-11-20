using UnityEngine;

namespace SpaceGame
{
    public static class Utils
    {

        /// <summary>
        /// <para>Since Laser speed is constant no need to calculate relative speed of laser to get interception pos!</para>
        /// <para>Calculates interception point between two moving objects where chaser speed is known but chaser vector is not known(Angle to fire at * LaserSpeed"*Sort of*")</para>
        /// <para>Can use System.Math and doubles to make this formula NASA like precision.</para>
        /// </summary>
        /// <param name="PC">Turret position</param>
        /// <param name="SC">Speed of laser</param>
        /// <param name="PR">Target initial position</param>
        /// <param name="VR">Target velocity vector</param>
        /// <returns>Interception Point as World Position</returns>
        // https://discussions.unity.com/t/formula-to-calculate-a-position-to-fire-at/48516/6
        public static Vector3 CalculateInterceptionPoint3D(Vector3 PC, float SC, Vector3 PR, Vector3 VR)
        {
            // Distance between turret and target
            Vector3 D = PC - PR;

            // Scale of distance vector
            float d = D.magnitude;

            // Speed of target scale of VR
            float SR = VR.magnitude;

            // Quadratic EQUATION members = (ax)^2 + bx + c = 0

            float a = Mathf.Pow(SC, 2) - Mathf.Pow(SR, 2);

            float b = 2 * Vector3.Dot(D, VR);

            float c = -Vector3.Dot(D, D);

            if ((Mathf.Pow(b, 2) - (4 * (a * c))) < 0) //% The QUADRATIC FORMULA will not return a real number because sqrt(-value) is not a real number thus no interception
            {
                return Vector2.zero;//TODO: HERE, PREVENT TURRET FROM FIRING LASERS INSTEAD OF MAKING LASERS FIRE AT ZERO!
            }

            // Quadratic FORMULA = x = (  -b+sqrt( ((b)^2) * 4*a*c )  ) / 2a
            float t = (-(b) + Mathf.Sqrt(Mathf.Pow(b, 2) - (4 * (a * c)))) / (2 * a);//% x = time to reach interception point which is = t

            // Calculate point of interception as vector from calculating distance between target and interception by t * VelocityVector
            return ((t * VR) + PR);
        }

        public static float FindClosestPointOfApproach(Vector3 aPos1, Vector3 aSpeed1, Vector3 aPos2, Vector3 aSpeed2)
        {
            Vector3 PVec = aPos1 - aPos2;
            Vector3 SVec = aSpeed1 - aSpeed2;
            float d = SVec.sqrMagnitude;
            // if d is 0 then the distance between Pos1 and Pos2 is never changing
            // so there is no point of closest approach... return 0
            // 0 means the closest approach is now!
            if (d >= -0.0001f && d <= 0.0002f)
                return 0.0f;
            return (-Vector3.Dot(PVec, SVec) / d);
        }

        // https://discussions.unity.com/t/interception-finding-closest-possible-point-to-target-when-interceptor-is-too-slow/931308/3
        public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
        {
            Vector3 targetDir = aTargetPos - aInterceptorPos;

            float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
            float tSpeed2 = aTargetSpeed.sqrMagnitude;
            float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
            float targetDist2 = targetDir.sqrMagnitude;
            float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
            if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
                return Vector3.zero;
            float sqrt = Mathf.Sqrt(d);
            float S1 = (-fDot1 - sqrt) / targetDist2;
            float S2 = (-fDot1 + sqrt) / targetDist2;

            if (S1 < 0.0001f)
            {
                if (S2 < 0.0001f)
                    return Vector3.zero;
                else
                    return (S2) * targetDir + aTargetSpeed;
            }
            else if (S2 < 0.0001f)
                return (S1) * targetDir + aTargetSpeed;
            else if (S1 < S2)
                return (S2) * targetDir + aTargetSpeed;
            else
                return (S1) * targetDir + aTargetSpeed;
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
            return targetPosition + t * targetRelativeVelocity;
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
