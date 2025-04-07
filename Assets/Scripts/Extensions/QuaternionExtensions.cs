using UnityEngine;

namespace SpaceGame.Extensions
{
    public static class QuaternionExtensions
    {

        public static Quaternion RotateXAxisTowards(this Quaternion from, Quaternion to, float maxDegreesDelta)
        {
            return RotateAxisTowards(from, to, maxDegreesDelta, Axis.X);
        }

        public static Quaternion RotateYAxisTowards(this Quaternion from, Quaternion to, float maxDegreesDelta)
        {
            return RotateAxisTowards(from, to, maxDegreesDelta, Axis.Y);
        }

        public static Quaternion RotateZAxisTowards(this Quaternion from, Quaternion to, float maxDegreesDelta)
        {
            return RotateAxisTowards(from, to, maxDegreesDelta, Axis.Z);
        }

        /// <summary>
        /// Rotates from one quaternion to another, modifying only a specified axis by a given angle limit.
        /// </summary>
        /// <param name="from">The starting quaternion.</param>
        /// <param name="to">The target quaternion.</param>
        /// <param name="maxDegreesDelta">The maximum allowed change in degrees per call.</param>
        /// <param name="axis">The axis to modify (X, Y, or Z).</param>
        /// <returns>A quaternion rotated towards the target, changing only the specified axis.</returns>
        public static Quaternion RotateAxisTowards(this Quaternion from, Quaternion to, float maxDegreesDelta, Axis axis)
        {
            // Convert both quaternions to Euler angles
            Vector3 fromEuler = from.eulerAngles;
            Vector3 toEuler = to.eulerAngles;

            // Create a modified target Euler vector based on the chosen axis
            Vector3 targetEuler = fromEuler; // Start with the 'from' Euler angles

            switch (axis)
            {
                case Axis.X:
                    targetEuler.x = Mathf.MoveTowardsAngle(fromEuler.x, toEuler.x, maxDegreesDelta);
                    break;
                case Axis.Y:
                    targetEuler.y = Mathf.MoveTowardsAngle(fromEuler.y, toEuler.y, maxDegreesDelta);
                    break;
                case Axis.Z:
                    targetEuler.z = Mathf.MoveTowardsAngle(fromEuler.z, toEuler.z, maxDegreesDelta);
                    break;

            }

            // Return the quaternion constructed from the modified Euler angles
            return Quaternion.Euler(targetEuler);
        }
    }
}
