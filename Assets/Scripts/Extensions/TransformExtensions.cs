using UnityEngine;

namespace SpaceGame.Extensions
{

    public static class TransformExtensions
    {

        public static void LookXAxisAt(this Transform transform, Vector3 target) => LookAxisAt(transform, target, Axis.X);

        public static void LookYAxisAt(this Transform transform, Vector3 target) => LookAxisAt(transform, target, Axis.Y);

        public static void LookZAxisAt(this Transform transform, Vector3 target) => LookAxisAt(transform, target, Axis.Z);

        public static void LookAxisAt(this Transform transform, Vector3 target, Axis axis)
        {
            // Obtain relative position of the target relative to this transform.
            // But in the local transform space of the this transform.
            var relativePosition = transform.InverseTransformDirection(target);

            // Remove local difference in the axis direction
            switch (axis)
            {
                case Axis.X:
                    relativePosition.x = 0f;
                    break;
                case Axis.Y:
                    relativePosition.y = 0f;
                    break;
                case Axis.Z:
                    relativePosition.z = 0f;
                    break;
            }

            // Convert it back to world space since LookAt expects a world position.
            var targetPosition = transform.TransformPoint(relativePosition);

            transform.transform.LookAt(targetPosition, transform.transform.up);
        }
    }
}
