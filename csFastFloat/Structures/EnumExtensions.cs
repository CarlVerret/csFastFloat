using System.Globalization;

namespace csFastFloat.Structures
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Evaluate enum flag without Enum.HasFlag, because it boxes enum values
        /// </summary>
        public static bool IsSet(this NumberStyles input, NumberStyles flag)
        {
            return (input & flag) == flag;
        }
    }
}