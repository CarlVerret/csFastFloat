using csFastFloat.Structures;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{
  public class FloatParser : FastParser<float>, IBinaryFormat<float>
  {
    unsafe internal override float ToFloat(bool negative, AdjustedMantissa am)
    {
      float f;
      ulong word = am.mantissa;
      word |= (ulong)(am.power2) << mantissa_explicit_bits();
      word = negative ? word | ((ulong)(1) << sign_index()) : word;

      fixed (ulong* p = &am.mantissa)
      {
        Buffer.MemoryCopy(p, &f, sizeof(float), sizeof(float));
      }

      return f;
    }

    public override int mantissa_explicit_bits() => 23;

    public override int minimum_exponent() => -127;

    public override int infinite_power() => 0xFF;

    public override int sign_index() => 31;

    public override int min_exponent_fast_path() => -10;

    public override int max_exponent_fast_path() => 10;

    public override int max_exponent_round_to_even() => 10;

    public override int min_exponent_round_to_even() => -17;

    public override ulong max_mantissa_fast_path() => (ulong)2 << mantissa_explicit_bits();

    public override int largest_power_of_ten() => 38;

    public override int smallest_power_of_ten() => 65;

    public override float exact_power_of_ten(long power) => Constants.powers_of_ten_float[power];

    internal override float FastPath(ParsedNumberString pns)
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

    public override float NaN() => float.NaN;

    public override float PositiveInfinity() => float.PositiveInfinity;

    public override float NegativeInfinity() => float.NegativeInfinity;
  }
}