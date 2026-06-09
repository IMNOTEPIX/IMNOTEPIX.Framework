using GTA.Math;
using GTA.Native;

namespace IMNOTEPIX.Framework.fCore
{
    public class Math
    {
        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            double pitch = rotation.X * System.Math.PI / 180.0;
            double yaw = rotation.Z * System.Math.PI / 180.0;

            return new Vector3(
                (float)(-System.Math.Sin(yaw) * System.Math.Abs(System.Math.Cos(pitch))),
                (float)(System.Math.Cos(yaw) * System.Math.Abs(System.Math.Cos(pitch))),
                (float)(System.Math.Sin(pitch))
            );
        }

        /// <summary>
        /// Clamps <paramref name="value"/> to [min, max].
        /// Note: .NET's Math.Clamp&lt;float&gt; does the same thing — prefer that
        /// for new code. This exists for callsites that predate .NET Standard 2.1.
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) 
                return min;
            if (value > max) 
                return max;
            return value;
        }

        /// <summary>
        /// Returns the ceiling of <paramref name="value"/> via the game's CEIL native.
        /// Equivalent to (int)Math.Ceiling(value) for most inputs.
        /// </summary>
        public static int Ceil(float value)
            => Function.Call<int>(Hash.CEIL, value);
    }
}
