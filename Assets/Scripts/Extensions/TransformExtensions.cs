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
            var previousAngles = transform.rotation.eulerAngles;
            transform.LookAt(target);
            transform.rotation = Quaternion.Euler(
                axis == Axis.X ? transform.eulerAngles.x : previousAngles.x,
                axis == Axis.Y ? transform.eulerAngles.y : previousAngles.y,
                axis == Axis.Z ? transform.eulerAngles.z : previousAngles.z
            );
        }
    }
}
