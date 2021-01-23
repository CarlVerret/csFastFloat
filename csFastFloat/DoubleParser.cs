using csFastFloat.Structures;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{
  public class DoubleParser : FastParser<double>, IBinaryFormat<double>
  {
    unsafe internal override double ToFloat(bool negative, AdjustedMantissa am)
    {
      double d;
      ulong word = am.mantissa;
      word |= (ulong)(am.power2) << mantissa_explicit_bits();
      word = negative ? word | ((ulong)(1) << sign_index()) : word;

      Buffer.MemoryCopy(&word, &d, sizeof(double), sizeof(double));

      return d;
    }

    public override int mantissa_explicit_bits() => 52;

    public override int minimum_exponent() => -1023;

    public override int infinite_power() => 0x7FF;

    public override int sign_index() => 63;

    public override int min_exponent_fast_path() => -22;

    public override int max_exponent_fast_path() => 22;

    public override int max_exponent_round_to_even() => 23;

    public override int min_exponent_round_to_even() => -4;

    public override ulong max_mantissa_fast_path() => (ulong)2 << mantissa_explicit_bits();

    public override int largest_power_of_ten() => 308;

    public override int smallest_power_of_ten() => -342;

    public override double exact_power_of_ten(long power) => Constants.powers_of_ten_double[power];

    internal override double FastPath(ParsedNumberString pns)
    {
      double value = (double)pns.mantissa;
      if (pns.exponent < 0)
      {
        value = value / exact_power_of_ten(-pns.exponent);
      }
      else
      {
        value = value * exact_power_of_ten(pns.exponent);
      }
      if (pns.negative) { value = -value; }
      return value;
    }

    public override double NaN() => double.NaN;

    public override double PositiveInfinity() => double.PositiveInfinity;

    public override double NegativeInfinity() => double.NegativeInfinity;
  }
}