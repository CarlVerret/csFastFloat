using csFastFloat.Structures;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{
  public sealed class FloatBinaryConstants
  {
    public const int mantissa_explicit_bits= 23;

    public const int minimum_exponent= -127;

    public const int infinite_power= 0xFF;

    public const int sign_index= 31;

    public const int min_exponent_fast_path= -10;

    public const int max_exponent_fast_path= 10;

    public const int max_exponent_round_to_even= 10;

    public const int min_exponent_round_to_even= -17;

    public const ulong max_mantissa_fast_path= (ulong)2 << mantissa_explicit_bits;

    public const int largest_power_of_ten= 38;

    public const int smallest_power_of_ten= -65;
  
    public const float NaN = float.NaN;

    public const float PositiveInfinity = float.PositiveInfinity;

    public const float NegativeInfinity = float.NegativeInfinity;
    
  }
}