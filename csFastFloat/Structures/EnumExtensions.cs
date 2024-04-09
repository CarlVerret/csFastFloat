using System.Globalization;

namespace csFastFloat.Structures
{
  /// <summary>
  /// Extension class for enums.  Much better performance than Enum's HasFlag for older .net frameworks.
  /// Credit Rene Brück
  /// </summary>
  public static class EnumExtensions
  {
    /// <summary>
    /// Evaluate enum flag without Enum.HasFlag, because it boxes enum values
    /// </summary>
    /// <param name="input">current enum to analyze</param>
    /// <param name="flag">flag to verify</param>
    /// <returns></returns>
    public static bool IsSet(this NumberStyles input, NumberStyles flag)
    {
      return (input & flag) == flag;
    }
  }
}