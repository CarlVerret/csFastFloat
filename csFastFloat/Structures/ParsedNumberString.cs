namespace csFastFloat.Structures
{
  public unsafe struct ParsedNumberString
  {
    internal long exponent;
    internal ulong mantissa;

    // internal char* lastmatch;
    internal bool negative;

    internal bool valid;
    internal bool too_many_digits;
  };
}