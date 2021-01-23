using csFastFloat.Structures;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{
  public class DoubleBinaryFormat : IBinaryFormat<double>
  {
    public int mantissa_explicit_bits() => 52;

    public int minimum_exponent() => -1023;

    public int infinite_power() => 0x7FF;

    public int sign_index() => 63;

    public int min_exponent_fast_path() => -22;

    public int max_exponent_fast_path() => 22;

    public int max_exponent_round_to_even() => 23;

    public int min_exponent_round_to_even() => -4;

    public ulong max_mantissa_fast_path() => (ulong)2 << mantissa_explicit_bits();

    public int largest_power_of_ten() => 308;

    public int smallest_power_of_ten() => -342;

    public double exact_power_of_ten(long power) => Constants.powers_of_ten_double[power];

    public double NaN() => double.NaN;

    public double PositiveInfinity() => double.PositiveInfinity;

    public double NegativeInfinity() => double.NegativeInfinity;

    public double ToFloat(bool negative, AdjustedMantissa am)
    {
      double d;
      ulong word = am.mantissa;
      word |= (ulong)(am.power2) << mantissa_explicit_bits();
      word = negative ? word | ((ulong)(1) << sign_index()) : word;

      unsafe
      {
        Buffer.MemoryCopy(&word, &d, sizeof(double), sizeof(double));
      }

      return d;
    }

    public double FastPath(ParsedNumberString pns)
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
  }
}