using csFastFloat.Structures;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{
  public sealed class FloatBinaryFormat : IBinaryFormat<float>
  {
    public int mantissa_explicit_bits() => 23;

    public int minimum_exponent() => -127;

    public int infinite_power() => 0xFF;

    public int sign_index() => 31;

    public int min_exponent_fast_path() => -10;

    public int max_exponent_fast_path() => 10;

    public int max_exponent_round_to_even() => 10;

    public int min_exponent_round_to_even() => -17;

    public ulong max_mantissa_fast_path() => (ulong)2 << mantissa_explicit_bits();

    public int largest_power_of_ten() => 38;

    public int smallest_power_of_ten() => -65;

    public float exact_power_of_ten(long power) => Constants.powers_of_ten_float[power];

    public float NaN() => float.NaN;

    public float PositiveInfinity() => float.PositiveInfinity;

    public float NegativeInfinity() => float.NegativeInfinity;

    public float ToFloat(bool negative, AdjustedMantissa am)
    {
      float f;
      ulong word = am.mantissa;
      word |= (ulong)(am.power2) << mantissa_explicit_bits();
      word = negative ? word | ((ulong)(1) << sign_index()) : word;

      unsafe
      {
        Buffer.MemoryCopy(&word, &f, sizeof(float), sizeof(float));
      }

      return f;
    }

    float IBinaryFormat<float>.FastPath(ParsedNumberString pns)
    {
      float value = (float)pns.mantissa;
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