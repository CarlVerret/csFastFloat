namespace csFastFloat.Structures
{
  internal class AdjustedMantissa
  {
    internal ulong mantissa;
    internal int power2; // a negative value indicates an invalid result

    public static bool operator ==(AdjustedMantissa a, AdjustedMantissa b)
            => a.mantissa == b.mantissa && a.power2 == b.power2;

    public static bool operator !=(AdjustedMantissa a, AdjustedMantissa b)
       => a.mantissa != b.mantissa || a.power2 != b.power2;

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();
  }
}