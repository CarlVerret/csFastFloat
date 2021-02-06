using System.Diagnostics.CodeAnalysis;

namespace csFastFloat.Structures
{
  public sealed class  AdjustedMantissa
  {
    internal ulong mantissa;
    internal int power2; // a negative value indicates an invalid result

    public static bool operator ==(AdjustedMantissa a, AdjustedMantissa b)
            => a.mantissa == b.mantissa && a.power2 == b.power2;

    public static bool operator !=(AdjustedMantissa a, AdjustedMantissa b)
       => a.mantissa != b.mantissa || a.power2 != b.power2;

    [ExcludeFromCodeCoverage]
    public override bool Equals(object obj) => base.Equals(obj);

    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => base.GetHashCode();
  }
}