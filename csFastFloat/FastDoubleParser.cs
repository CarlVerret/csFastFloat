﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using csFastFloat.Structures;
using System.Globalization;

namespace csFastFloat
{



  public static class FastDoubleParser
  {
    private static void ThrowArgumentException() => throw new ArgumentException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal  static double exact_power_of_ten(long power)
    {
#if NET5_0
      Debug.Assert(power < Constants.powers_of_ten_double.Length);
      ref double tableRef = ref MemoryMarshal.GetArrayDataReference(Constants.powers_of_ten_double);
      return Unsafe.Add(ref tableRef, (nint)power);
#else
      return Constants.powers_of_ten_double[power];
#endif

    }

    internal static double ToFloat(bool negative, AdjustedMantissa am)
    {
      ulong word = am.mantissa;
      word |= (ulong)(uint)(am.power2) << DoubleBinaryConstants.mantissa_explicit_bits;
      word = negative ? word | ((ulong)(1) << DoubleBinaryConstants.sign_index) : word;

      return BitConverter.Int64BitsToDouble((long)word);
    }

    internal  static double FastPath(ParsedNumberString pns)
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


    public static unsafe double ParseDouble(string s, NumberStyles expectedFormat = NumberStyles.Float, char decimal_separator = '.')
    {
      if (s == null)
        ThrowArgumentNull();
        static void ThrowArgumentNull() => throw new ArgumentNullException(nameof(s));

      fixed (char* pStart = s)
      {
        return ParseDouble(pStart, pStart + (uint)s.Length, expectedFormat, decimal_separator);
      }
    }

    public static unsafe double ParseDouble(string s, out int characters_consumed, NumberStyles expectedFormat = NumberStyles.Float, char decimal_separator = '.')
    {
      if (s == null)
        ThrowArgumentNull();
        static void ThrowArgumentNull() => throw new ArgumentNullException(nameof(s));

      fixed (char* pStart = s)
      {
        return ParseNumber(pStart, pStart + (uint)s.Length, out characters_consumed, expectedFormat, decimal_separator);
      }
    }


    public static unsafe double ParseDouble(ReadOnlySpan<char> s, NumberStyles expectedFormat = NumberStyles.Float, char decimal_separator = '.')
    {
      fixed (char* pStart = s)
      {
        return ParseDouble(pStart, pStart + (uint)s.Length, expectedFormat, decimal_separator);
      }
    }

    public static unsafe double ParseDouble(ReadOnlySpan<char> s, out int characters_consumed, NumberStyles expectedFormat = NumberStyles.Float, char decimal_separator = '.')
    {
      fixed (char* pStart = s)
      {
        return ParseNumber(pStart, pStart + (uint)s.Length, out characters_consumed, expectedFormat, decimal_separator);
      }
    }

    unsafe static public double ParseDouble(char* first, char* last, NumberStyles expectedFormat = NumberStyles.Float, char decimal_separator = '.')
      => ParseNumber(first, last, out int _, expectedFormat, decimal_separator);


    unsafe static internal double ParseNumber(char* first, char* last, out int characters_consumed, NumberStyles expectedFormat = NumberStyles.Float, char decimal_separator = '.')
    {
      var leading_spaces = 0;
      while ((first != last) && Utils.is_ascii_space(*first))
      {
        first++;
        leading_spaces++;
      }
      if (first == last)
      {
        ThrowArgumentException();
      }
      ParsedNumberString pns = ParsedNumberString.ParseNumberString(first, last, expectedFormat);
      if (!pns.valid)
      {
        return HandleInvalidInput(first, last, out characters_consumed);
      }
      characters_consumed = pns.characters_consumed + leading_spaces;

      // Next is Clinger's fast path.
      if (DoubleBinaryConstants.min_exponent_fast_path <= pns.exponent && pns.exponent <= DoubleBinaryConstants.max_exponent_fast_path && pns.mantissa <= DoubleBinaryConstants.max_mantissa_fast_path && !pns.too_many_digits)
      {
        return FastPath(pns);
      }

      AdjustedMantissa am = ComputeFloat(pns.exponent, pns.mantissa);
      if (pns.too_many_digits)
      {
        if (am != ComputeFloat(pns.exponent, pns.mantissa + 1))
        {
          am.power2 = -1; // value is invalid.
        }
      }
      // If we called compute_float<binary_format<T>>(pns.exponent, pns.mantissa) and we have an invalid power (am.power2 < 0),
      // then we need to go the long way around again. This is very uncommon.
      if (am.power2 < 0) { am = ParseLongMantissa(first, last, decimal_separator); }
      return ToFloat(pns.negative, am);
    }

    internal unsafe static  double ParseNumber (byte* first, byte* last, out int characters_consumed, NumberStyles expectedFormat = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      while ((first != last) && Utils.is_space(*first))
      {
        first++;
      }
      if (first == last)
      {
        ThrowArgumentException();
      }
      ParsedNumberString pns = ParsedNumberString.ParseNumberString(first, last, expectedFormat);
      if (!pns.valid)
      {
        return HandleInvalidInput(first, last, out characters_consumed);
      }
      characters_consumed = pns.characters_consumed;

      // Next is Clinger's fast path.
      if (DoubleBinaryConstants.min_exponent_fast_path <= pns.exponent && pns.exponent <= DoubleBinaryConstants.max_exponent_fast_path && pns.mantissa <= DoubleBinaryConstants.max_mantissa_fast_path && !pns.too_many_digits)
      {
        return FastPath(pns);
      }

      AdjustedMantissa am = ComputeFloat(pns.exponent, pns.mantissa);
      if (pns.too_many_digits)
      {
        if (am != ComputeFloat(pns.exponent, pns.mantissa + 1))
        {
          am.power2 = -1; // value is invalid.
        }
      }
      // If we called compute_float<binary_format<T>>(pns.exponent, pns.mantissa) and we have an invalid power (am.power2 < 0),
      // then we need to go the long way around again. This is very uncommon.
      if (am.power2 < 0) { am = ParseLongMantissa(first, last, decimal_separator); }
      return ToFloat(pns.negative, am);
    }

    public static unsafe double ParseDouble(ReadOnlySpan<byte> s, NumberStyles expectedFormat = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      fixed(byte* pStart = s)
      {
        return ParseNumber(pStart, pStart + s.Length, out int _, expectedFormat, decimal_separator);
      }
    }
    public static unsafe double ParseDouble(ReadOnlySpan<byte> s, out int characters_consumed, NumberStyles expectedFormat = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      fixed(byte* pStart = s)
      {
        return ParseNumber(pStart, pStart + s.Length, out characters_consumed, expectedFormat, decimal_separator);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="q"></param>
    /// <param name="w"></param>
    ///
    /// <returns></returns>

    internal static AdjustedMantissa ComputeFloat (long q, ulong w)
    {
      var answer = new AdjustedMantissa();

      if ((w == 0) || (q < DoubleBinaryConstants.smallest_power_of_ten))
      {
        // result should be zero
        return default;
      }
      if (q > DoubleBinaryConstants.largest_power_of_ten)
      {
        // we want to get infinity:
        answer.power2 = DoubleBinaryConstants.infinite_power;
        answer.mantissa = 0;
        return answer;
      }
      // At this point in time q is in [smallest_power_of_five, largest_power_of_five].

      // We want the most significant bit of i to be 1. Shift if needed.
      int lz = Utils.LeadingZeroCount(w);
      w <<= lz;

      // The required precision is mantissa_explicit_bits() + 3 because
      // 1. We need the implicit bit
      // 2. We need an extra bit for rounding purposes
      // 3. We might lose a bit due to the "upperbit" routine (result too small, requiring a shift)

      value128 product = Utils.compute_product_approximation(DoubleBinaryConstants.mantissa_explicit_bits + 3, q, w);
      if (product.low == 0xFFFFFFFFFFFFFFFF)
      { //  could guard it further
        // In some very rare cases, this could happen, in which case we might need a more accurate
        // computation that what we can provide cheaply. This is very, very unlikely.
        //
        bool inside_safe_exponent = (q >= -27) && (q <= 55); // always good because 5**q <2**128 when q>=0,
                                                             // and otherwise, for q<0, we have 5**-q<2**64 and the 128-bit reciprocal allows for exact computation.
        if (!inside_safe_exponent)
        {
          answer.power2 = -1; // This (a negative value) indicates an error condition.
          return answer;
        }
      }
      // The "compute_product_approximation" function can be slightly slower than a branchless approach:
      // value128 product = compute_product(q, w);
      // but in practice, we can win big with the compute_product_approximation if its additional branch
      // is easily predicted. Which is best is data specific.
      int upperbit = (int)(product.high >> 63);

      answer.mantissa = product.high >> (upperbit + 64 - DoubleBinaryConstants.mantissa_explicit_bits - 3);

      answer.power2 = (int)(Utils.power((int)(q)) + upperbit - lz - DoubleBinaryConstants.minimum_exponent);
      if (answer.power2 <= 0)
      { // we have a subnormal?
        // Here have that answer.power2 <= 0 so -answer.power2 >= 0
        if (-answer.power2 + 1 >= 64)
        { // if we have more than 64 bits below the minimum exponent, you have a zero for sure.
          answer.power2 = 0;
          answer.mantissa = 0;
          // result should be zero
          return answer;
        }
        // next line is safe because -answer.power2 + 1 < 64
        answer.mantissa >>= -answer.power2 + 1;
        // Thankfully, we can't have both "round-to-even" and subnormals because
        // "round-to-even" only occurs for powers close to 0.
        answer.mantissa += (answer.mantissa & 1); // round up
        answer.mantissa >>= 1;
        // There is a weird scenario where we don't have a subnormal but just.
        // Suppose we start with 2.2250738585072013e-308, we end up
        // with 0x3fffffffffffff x 2^-1023-53 which is technically subnormal
        // whereas 0x40000000000000 x 2^-1023-53  is normal. Now, we need to round
        // up 0x3fffffffffffff x 2^-1023-53  and once we do, we are no longer
        // subnormal, but we can only know this after rounding.
        // So we only declare a subnormal if we are smaller than the threshold.
        answer.power2 = (answer.mantissa < ((ulong)(1) << DoubleBinaryConstants.mantissa_explicit_bits)) ? 0 : 1;
        return answer;
      }

      // usually, we round *up*, but if we fall right in between and and we have an
      // even basis, we need to round down
      // We are only concerned with the cases where 5**q fits in single 64-bit word.
      if ((product.low <= 1) && (q >= DoubleBinaryConstants.min_exponent_round_to_even) && (q <= DoubleBinaryConstants.max_exponent_round_to_even) &&
          ((answer.mantissa & 3) == 1))
      { // we may fall between two floats!
        // To be in-between two floats we need that in doing
        //   answer.mantissa = product.high >> (upperbit + 64 - mantissa_explicit_bits() - 3);
        // ... we dropped out only zeroes. But if this happened, then we can go back!!!
        if ((answer.mantissa << (upperbit + 64 - DoubleBinaryConstants.mantissa_explicit_bits - 3)) == product.high)
        {
          answer.mantissa &= ~(ulong)(1);          // flip it so that we do not round up
        }
      }

      answer.mantissa += (answer.mantissa & 1); // round up
      answer.mantissa >>= 1;
      if (answer.mantissa >= ((ulong)(2) << DoubleBinaryConstants.mantissa_explicit_bits))
      {
        answer.mantissa = ((ulong)(1) << DoubleBinaryConstants.mantissa_explicit_bits);
        answer.power2++; // undo previous addition
      }

      answer.mantissa &= ~((ulong)(1) << DoubleBinaryConstants.mantissa_explicit_bits);
      if (answer.power2 >= DoubleBinaryConstants.infinite_power)
      { // infinity
        answer.power2 = DoubleBinaryConstants.infinite_power;
        answer.mantissa = 0;
      }
      return answer;
    }


    internal static AdjustedMantissa ComputeFloat(DecimalInfo d)
    {
      AdjustedMantissa answer = new AdjustedMantissa();
      if (d.num_digits == 0)
      {
        // should be zero
        return default;
      }
      // At this point, going further, we can assume that d.num_digits > 0.
      //
      // We want to guard against excessive decimal point values because
      // they can result in long running times. Indeed, we do
      // shifts by at most 60 bits. We have that log(10**400)/log(2**60) ~= 22
      // which is fine, but log(10**299995)/log(2**60) ~= 16609 which is not
      // fine (runs for a long time).
      //
      if (d.decimal_point < -324)
      {
        // We have something smaller than 1e-324 which is always zero
        // in binary64 and binary32.
        // It should be zero.
        return default;
      }
      else if (d.decimal_point >= 310)
      {
        // We have something at least as large as 0.1e310 which is
        // always infinite.
        answer.power2 = DoubleBinaryConstants.infinite_power;
        answer.mantissa = 0;
        return answer;
      }
      const int max_shift = 60;
      const uint num_powers = 19;

      int exp2 = 0;
      while (d.decimal_point > 0)
      {
        uint n = (uint)(d.decimal_point);
        int shift = (n < num_powers) ? Constants.get_powers(n) : max_shift;

        d.decimal_right_shift(shift);
        if (d.decimal_point < -Constants.decimal_point_range)
        {
          // should be zero
          answer.power2 = 0;
          answer.mantissa = 0;
          return answer;
        }
        exp2 += (int)(shift);
      }
      // We shift left toward [1/2 ... 1].
      while (d.decimal_point <= 0)
      {
        int shift;
        if (d.decimal_point == 0)
        {
          if (d.digits[0] >= 5)
          {
            break;
          }
          if (d.digits[0] < 2)
          { shift = 2; }
          else { shift = 1; }
        }
        else
        {
          uint n = (uint)(-d.decimal_point);
          shift = (n < num_powers) ? Constants.get_powers(n) : max_shift;
        }

        d.decimal_left_shift(shift);

        if (d.decimal_point > Constants.decimal_point_range)
        {
          // we want to get infinity:
          answer.power2 = DoubleBinaryConstants.infinite_power;
          answer.mantissa = 0;
          return answer;
        }
        exp2 -= (int)(shift);
      }
      // We are now in the range [1/2 ... 1] but the binary format uses [1 ... 2].
      exp2--;

      int min_exp = DoubleBinaryConstants.minimum_exponent;

      while ((min_exp + 1) > exp2)
      {
        int n = (int)((min_exp + 1) - exp2);
        if (n > max_shift)
        {
          n = max_shift;
        }
        d.decimal_right_shift(n);
        exp2 += (int)(n);
      }
      if ((exp2 - min_exp) >= DoubleBinaryConstants.infinite_power)
      {
        answer.power2 = DoubleBinaryConstants.infinite_power;
        answer.mantissa = 0;
        return answer;
      }

      int mantissa_size_in_bits = DoubleBinaryConstants.mantissa_explicit_bits + 1;
      d.decimal_left_shift((int)mantissa_size_in_bits);

      ulong mantissa = d.round();
      // It is possible that we have an overflow, in which case we need
      // to shift back.
      if (mantissa >= ((ulong)(1) << mantissa_size_in_bits))
      {
        d.decimal_right_shift(1);
        exp2 += 1;
        mantissa = d.round();
        if ((exp2 - min_exp) >= DoubleBinaryConstants.infinite_power)
        {
          answer.power2 = DoubleBinaryConstants.infinite_power;
          answer.mantissa = 0;
          return answer;
        }
      }
      answer.power2 = exp2 - min_exp;
      if (mantissa < ((ulong)(1) << DoubleBinaryConstants.mantissa_explicit_bits)) { answer.power2--; }
      answer.mantissa = mantissa & (((ulong)(1) << DoubleBinaryConstants.mantissa_explicit_bits) - 1);

      return answer;
    }

    // UTF-16 inputs
    unsafe static internal AdjustedMantissa ParseLongMantissa(char* first, char* last,  char decimal_separator)
    {
      DecimalInfo d = DecimalInfo.parse_decimal(first, last, decimal_separator);
      return ComputeFloat(d);
    }

    // UTF-8/ASCII inputs
    unsafe static internal AdjustedMantissa ParseLongMantissa(byte* first, byte* last,  byte decimal_separator)
    {
      DecimalInfo d = DecimalInfo.parse_decimal(first, last, decimal_separator);
      return ComputeFloat(d);
    }


    unsafe static internal double HandleInvalidInput(char* first, char* last, out int characters_consumed)
    {
      if (last - first >= 3)
      {
        if (Utils.strncasecmp(first, "nan", 3))
        {
          characters_consumed = 3;
          return DoubleBinaryConstants.NaN;
        }
        if (Utils.strncasecmp(first, "inf", 3))
        {
          if ((last - first >= 8) && Utils.strncasecmp(first, "infinity", 8))
          {
            characters_consumed = 8;
            return DoubleBinaryConstants.PositiveInfinity;
          }
          characters_consumed = 3;
          return DoubleBinaryConstants.PositiveInfinity;
        }
        if (last - first >= 4)
        {
          if (Utils.strncasecmp(first, "+nan", 4) || Utils.strncasecmp(first, "-nan", 4))
          {
            characters_consumed = 4;
            return DoubleBinaryConstants.NaN;
          }
          if (Utils.strncasecmp(first, "+inf", 4) ||
              Utils.strncasecmp(first, "-inf", 4))
          {
            if((last - first >= 9) && Utils.strncasecmp(first + 1, "infinity", 8))
            {
              characters_consumed = 9;
            } else {
              characters_consumed = 4;
            }
            return (first[0] == '-') ? DoubleBinaryConstants.NegativeInfinity : DoubleBinaryConstants.PositiveInfinity;
          }
        }
      }
      ThrowArgumentException();
      characters_consumed = 0;
      return 0d;
    }


    unsafe static internal double HandleInvalidInput(byte* first, byte* last, out int characters_consumed)
    {
      // C# does not (yet) allow literal ASCII strings (it uses UTF-16), so
      // we need to use byte arrays.
      // "infinity"  string in ASCII, e.g., 105 = i
      ReadOnlySpan<byte> infinity_string = new byte[]{105, 110, 102, 105, 110, 105, 116, 121};
      // "inf" string in ASCII
      ReadOnlySpan<byte> inf_string = new byte[]{105, 110, 102};
      // "+inf" string in ASCII
      ReadOnlySpan<byte> pinf_string = new byte[]{43, 105, 110, 102};
      // "-inf" string in ASCII
      ReadOnlySpan<byte> minf_string = new byte[]{5, 105, 110, 102};
      // "nan" string in ASCII
      ReadOnlySpan<byte> nan_string = new byte[]{110, 97, 110};
      // "-nan" string in ASCII
      ReadOnlySpan<byte> mnan_string = new byte[]{45, 110, 97, 110};
      // "+nan" string in ASCII
      ReadOnlySpan<byte> pnan_string = new byte[]{43, 110, 97, 110};

      if (last - first >= 3)
      {
        if (Utils.strncasecmp(first, nan_string, 3))
        {
          characters_consumed = 3;
          return DoubleBinaryConstants.NaN;
        }
        if (Utils.strncasecmp(first, inf_string, 3))
        {
          if ((last - first >= 8) && Utils.strncasecmp(first, infinity_string, 8))
          {
            characters_consumed = 8;
            return DoubleBinaryConstants.PositiveInfinity;
          }
          characters_consumed = 3;
          return DoubleBinaryConstants.PositiveInfinity;
        }
        if (last - first >= 4)
        {
          if (Utils.strncasecmp(first, pnan_string, 4) || Utils.strncasecmp(first, mnan_string, 4))
          {
            characters_consumed = 4;
            return DoubleBinaryConstants.NaN;
          }
          if (Utils.strncasecmp(first, pinf_string, 4) ||
              Utils.strncasecmp(first, minf_string, 4))
          {
            if((last - first >= 9) && Utils.strncasecmp(first + 1, infinity_string, 8))
            {
              characters_consumed = 9;
            } else {
              characters_consumed = 4;
            }
            return (first[0] == '-') ? DoubleBinaryConstants.NegativeInfinity : DoubleBinaryConstants.PositiveInfinity;
          }
        }
      }
      ThrowArgumentException();
      characters_consumed = 0;
      return 0d;
    }





  }



}