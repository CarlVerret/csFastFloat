using csFastFloat.Structures;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{

public static class DoubleBinaryConstants
{


    public const int mantissa_explicit_bits= 52;

    public const int minimum_exponent= -1023;

    public const int infinite_power= 0x7FF;

    public const int sign_index= 63;

    public const int min_exponent_fast_path= -22;

    public const int max_exponent_fast_path= 22;

    public const int max_exponent_round_to_even= 23;

    public const int min_exponent_round_to_even= -4;

    public const ulong max_mantissa_fast_path = (ulong)2 << 52;

    public const int largest_power_of_ten= 308;

    public const int smallest_power_of_ten= -342;

   
    public const double NaN= double.NaN;

    public const double PositiveInfinity= double.PositiveInfinity;

    public const double NegativeInfinity= double.NegativeInfinity;

  }
}

