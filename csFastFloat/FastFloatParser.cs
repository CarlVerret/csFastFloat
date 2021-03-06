﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using csFastFloat.Constants;
using csFastFloat.Structures;

namespace csFastFloat
{

  /// <summary>
  /// This class is intented to parse float values from inputs such as string, readonlyspans  and char pointers
  /// There's two set of functions, one for UTF-16 encoding another for UTF-8 encoding
  /// This is a C# port of Daniel Lemire's fast_float library written in C++
  /// https://github.com/fastfloat/fast_float
  /// </summary>
  public static class FastFloatParser
  {
    private static void ThrowArgumentException() => throw new ArgumentException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float exact_power_of_ten(long power)
    {
#if NET5_0
      Debug.Assert(power < CalculationConstants.powers_of_ten_float.Length);
      ref float tableRef = ref MemoryMarshal.GetArrayDataReference(CalculationConstants.powers_of_ten_float);
      return Unsafe.Add(ref tableRef, (nint)power);
#else
      return CalculationConstants.powers_of_ten_float[power];
#endif
    }

    /// <summary>
    /// Resolve the adjusted mantissa back to its corresponding float value
    /// </summary>
    /// <param name="negative">bool:  true indicates a negative value should be returned</param>
    /// <param name="am">adjusted mantissa (mantissa and exponent)</param>
    /// <returns>float value corresponding</returns>
    internal static float ToFloat(bool negative, AdjustedMantissa am)
    {
      ulong word = am.mantissa;
      word |= (ulong)(uint)(am.power2) << FloatBinaryConstants.mantissa_explicit_bits;
      word = negative ? word | ((ulong)(1) << FloatBinaryConstants.sign_index) : word;
      uint truncated_word = (uint) word;
      return Utils.Int32BitsToSingle((int)truncated_word);
    }

    /// <summary>
    /// Clinger's fast path
    /// </summary>
    /// <param name="pns">Parsed info of the input</param>
    /// <returns></returns>
    internal static float FastPath(ParsedNumberString pns)
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


    /// <summary>
    /// Try parsing a float from a UTF-16 encoded string in the given number style
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(string s, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
     => TryParseFloat(s, out _, out result, styles, decimal_separator);
  
    /// <summary>
    /// Try parsing a float from a UTF-16 encoded string in the given number style, counting number of consumed caracters
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="characters_consumed">number of consumed caracters while parsing </param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(string s, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
      => TryParseFloat(s.AsSpan(), out characters_consumed, out result, styles, decimal_separator);

    /// <summary>
    /// Try parsing a float from a UTF-16 encoded readonly span of chars in the given number style
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(ReadOnlySpan<char> s, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
      => TryParseFloat(s, out int _, out result, styles, decimal_separator);

    /// <summary>
    /// Try parsing a float from a UTF-16 encoded readonly span of chars in the given number style, counting number of consumed caracters
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="characters_consumed">number of consumed caracters while parsing </param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(ReadOnlySpan<char> s, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
    {
      fixed (char* pStart = s)
      {
        return TryParseFloat(pStart, pStart + (uint)s.Length, out characters_consumed, out result, styles, decimal_separator);
      }
    }

    /// <summary>
    /// Try parsing a float from a UTF-16 encoded input in the given number style
    /// </summary>
    /// <param name="first">char pointer to the begining of the input</param>
    /// <param name="last">char pointer to the end of the input</param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(char* first, char* last, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
      => TryParseFloat(first, last, out int _, out result, styles, decimal_separator);

    /// <summary>
    /// Try parsing a float from a UTF-16 encoded input in the given number style, counting number of consumed caracters
    /// </summary>
    /// <param name="first">char pointer to the begining of the input</param>
    /// <param name="last">char pointer to the end of the input</param>
    /// <param name="characters_consumed">number of consumed caracters while parsing </param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(char* first, char* last, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
     => TryParseNumber(first, last, out characters_consumed, out result, styles, decimal_separator);


    /// <summary>
    /// Try parsing a float from a UTF-8 encoded readonly span of bytes in the given number style
    /// </summary>
    /// <param name="s">readonly span of bytes</param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(ReadOnlySpan<byte> s, out float result, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
      => TryParseFloat(s, out int _, out result, styles, decimal_separator);

    /// <summary>
    /// Try parsing a float from a UTF-8 encoded readonly span of bytes in the given number style, counting number of consumed caracters
    /// </summary>
    /// <param name="s">readonly span of bytes</param>
    /// <param name="characters_consumed">number of consumed caracters while parsing </param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(ReadOnlySpan<byte> s, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      fixed (byte* pStart = s)
      {
        return TryParseFloat(pStart, pStart + s.Length, out characters_consumed, out result, styles, decimal_separator);
      }
    }


    /// <summary>
    /// Try parsing a float from a UTF-8 encoded inputin the given number style
    /// </summary>
    /// <param name="first">byte pointer to the begining of the input</param>
    /// <param name="last">byte pointer to the end of the input</param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(byte* first, byte* last, out float result, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
      => TryParseFloat(first, last, out int _, out result, styles, decimal_separator);

    /// <summary>
    /// Try parsing a float from a UTF-8 encoded inputin the given number style, counting number of consumed caracters
    /// </summary>
    /// <param name="first">byte pointer to the begining of the input</param>
    /// <param name="last">byte pointer to the end of the input</param>
    /// <param name="characters_consumed">number of consumed caracters while parsing </param>
    /// <param name="result">output float value</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool : true is sucessfuly parsed</returns>
    public static unsafe bool TryParseFloat(byte* first, byte* last, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
      => TryParseNumber(first, last, out characters_consumed, out result, styles, decimal_separator);

    /// <summary>
    /// Parses float from a UTF-16 encoded string in the given number style
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(string s, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
      => ParseFloat(s, out int _, styles, decimal_separator);

    /// <summary>
    /// Parses float from a UTF-16 encoded string in the given number style counting number of characters consumed
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="characters_consumed">number of character consumed while parsing</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(string s, out int characters_consumed, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
    {
      if (s == null)
        ThrowArgumentNull();
      static void ThrowArgumentNull() => throw new ArgumentNullException(nameof(s));

      fixed (char* pStart = s)
      {
        return ParseFloat(pStart, pStart + (uint)s.Length, out characters_consumed, styles, decimal_separator);
      }
    }

    /// <summary>
    /// Parses float from a UTF-16 encoded readonly span of chars in the given number style 
    /// </summary>
    /// <param name="s">readonly span of chars</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed double value </returns>
    public static unsafe float ParseFloat(ReadOnlySpan<char> s, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
      => ParseFloat(s, out int _, styles, decimal_separator);

    /// <summary>
    /// Parses float from a UTF-16 encoded readonly span of chars in the given number style counting number of characters consumed
    /// </summary>
    /// <param name="s">readonly span of chars</param>
    /// <param name="characters_consumed">number of character consumed while parsing</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(ReadOnlySpan<char> s, out int characters_consumed, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
    {
      fixed (char* pStart = s)
      {
        return ParseFloat(pStart, pStart + (uint)s.Length, out characters_consumed, styles, decimal_separator);
      }
    }

    /// <summary>
    /// Parses float from a UTF-16 encoded input in the given number style
    /// </summary>
    /// <param name="first">char pointer to the begining of the input</param>
    /// <param name="last">char pointer to the end of the input</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(char* first, char* last, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
      => ParseFloat(first, last, out int _, styles, decimal_separator);

    /// <summary>
    /// Parses float from a UTF-8 encoded readonly span of bytes in the given number
    /// </summary>
    /// <param name="s">readonly span of bytes</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(ReadOnlySpan<byte> s, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
      => ParseFloat(s, out int _, styles, decimal_separator);

    /// <summary>
    /// Parses float from a UTF-16 encoded string in the given number style counting number of characters consumed
    /// </summary>
    /// <param name="s">input string</param>
    /// <param name="characters_consumed">number of character consumed while parsing</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(ReadOnlySpan<byte> s, out int characters_consumed, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      fixed (byte* pStart = s)
      {
        return ParseFloat(pStart, pStart + s.Length, out characters_consumed, styles, decimal_separator);
      }
    }

    /// <summary>
    /// Parses float from a UTF-16 encoded input in the given number style counting number of characters consumed
    /// </summary>
    /// <param name="first">char pointer to the begining of the input</param>
    /// <param name="last">char pointer to the end of the input</param>
    /// <param name="characters_consumed">number of character consumed while parsing</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(char* first, char* last, out int characters_consumed, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
    {
      if (TryParseFloat(first, last, out characters_consumed, out float result, styles, decimal_separator))
      {
        return result;
      }
      
      ThrowArgumentException();
      throw null;

    }

    /// <summary>
    /// Parses float from a UTF-8 encoded input in the given number style counting number of characters consumed
    /// </summary>
    /// <param name="first">byte pointer to the begining of the input</param>
    /// <param name="last">byte pointer to the end of the input</param>
    /// <param name="characters_consumed">number of character consumed while parsing</param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>parsed float value </returns>
    public static unsafe float ParseFloat(byte* first, byte* last, out int characters_consumed, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      if (TryParseFloat(first, last, out characters_consumed, out float result, styles, decimal_separator))
      {
        return result;
      }
      ThrowArgumentException();
      throw null;
    }


    /// <summary>
    /// Try to parse the input (UTF-16) and compute the float value
    /// </summary>
    /// <param name="first">char pointer to the begining of the input</param>
    /// <param name="last">char pointer to the end of the input</param>
    /// <param name="characters_consumed">number of characters consumed while parsing</param>
    /// <param name="result">out float : parsed float value </param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// <returns>bool:  true indicates a succesful parsing</returns>
    internal static unsafe bool TryParseNumber(char* first, char* last, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, char decimal_separator = '.')
    {
      while ((first != last) && Utils.is_ascii_space(*first))
      {
        first++;
      }
      if (first == last)
      {
        result = 0;
        characters_consumed = 0;
        return false;
      }
      ParsedNumberString pns = ParsedNumberString.ParseNumberString(first, last, styles);
      if (!pns.valid)
      {

        var success = FastDoubleParser.TryHandleInvalidInput(first, last, out characters_consumed, out double temp) ;
        result = (float)temp;
        return success;
      }
      characters_consumed = pns.characters_consumed;

      // Next is Clinger's fast path.
      if (FloatBinaryConstants.min_exponent_fast_path <= pns.exponent && pns.exponent <= FloatBinaryConstants.max_exponent_fast_path && pns.mantissa <= FloatBinaryConstants.max_mantissa_fast_path && !pns.too_many_digits)
      {
        result = FastPath(pns);
        return true;
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
      result =  ToFloat(pns.negative, am); 
      return true;
    }

    /// <summary>
    /// Try to parse the input (UTF-8) and compute the float value
    /// </summary>
    /// <param name="first">byte pointer to the begining of the input</param>
    /// <param name="last">byte pointer to the end of the input</param>
    /// <param name="characters_consumed">number of characters consumed while parsing</param>
    /// <param name="result">out float : parsed float value </param>
    /// <param name="styles">allowed styles for the input string</param>
    /// <param name="decimal_separator">decimal separator to be used</param>
    /// /// <returns>bool:  true indicates a succesful parsing</returns>
    internal static unsafe bool TryParseNumber(byte* first, byte* last, out int characters_consumed, out float result, NumberStyles styles = NumberStyles.Float, byte decimal_separator = (byte)'.')
    {
      var leading_spaces = 0;

      while ((first != last) && Utils.is_space(*first))
      {
        first++;
        leading_spaces++;
      }
      if (first == last)
      {
        ThrowArgumentException();
      }
      ParsedNumberString pns = ParsedNumberString.ParseNumberString(first, last, styles);
      if (!pns.valid)
      {
        var success = FastDoubleParser.TryHandleInvalidInput(first, last, out characters_consumed, out double temp);
        result = (float)temp;
        return success;
      }
      characters_consumed = pns.characters_consumed+ leading_spaces;

      // Next is Clinger's fast path.
      if (FloatBinaryConstants.min_exponent_fast_path <= pns.exponent && pns.exponent <= FloatBinaryConstants.max_exponent_fast_path && pns.mantissa <= FloatBinaryConstants.max_mantissa_fast_path && !pns.too_many_digits)
      {
        result = FastPath(pns);
        return true;
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
      if (am.power2 < 0) { am = ParseLongMantissa(first, last, (byte)decimal_separator); }
      result = ToFloat(pns.negative, am);
      return true;
    }


    /// <summary>
    /// Daniel Lemire's Fast-float algorithm
    /// please refer to https://arxiv.org/abs/2101.11408
    /// </summary>
    /// <param name="q">exponemt</param>
    /// <param name="w">decimal mantissa</param>
    /// <returns>Adjusted mantissa</returns>
    internal static AdjustedMantissa ComputeFloat(long q, ulong w)
    {
      var answer = new AdjustedMantissa();

      if ((w == 0) || (q < FloatBinaryConstants.smallest_power_of_ten))
      {
        answer.power2 = 0;
        answer.mantissa = 0;
        // result should be zero
        return answer;
      }
      if (q > FloatBinaryConstants.largest_power_of_ten)
      {
        // we want to get infinity:
        answer.power2 = FloatBinaryConstants.infinite_power;
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

      value128 product = Utils.compute_product_approximation(FloatBinaryConstants.mantissa_explicit_bits + 3, q, w);
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

      answer.mantissa = product.high >> (upperbit + 64 - FloatBinaryConstants.mantissa_explicit_bits - 3);

      answer.power2 = (int)(Utils.power((int)(q)) + upperbit - lz - FloatBinaryConstants.minimum_exponent);
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
        answer.power2 = (answer.mantissa < ((ulong)(1) << FloatBinaryConstants.mantissa_explicit_bits)) ? 0 : 1;
        return answer;
      }

      // usually, we round *up*, but if we fall right in between and and we have an
      // even basis, we need to round down
      // We are only concerned with the cases where 5**q fits in single 64-bit word.
      if ((product.low <= 1) && (q >= FloatBinaryConstants.min_exponent_round_to_even) && (q <= FloatBinaryConstants.max_exponent_round_to_even) &&
          ((answer.mantissa & 3) == 1))
      { // we may fall between two floats!
        // To be in-between two floats we need that in doing
        //   answer.mantissa = product.high >> (upperbit + 64 - mantissa_explicit_bits() - 3);
        // ... we dropped out only zeroes. But if this happened, then we can go back!!!
        if ((answer.mantissa << (upperbit + 64 - FloatBinaryConstants.mantissa_explicit_bits - 3)) == product.high)
        {
          answer.mantissa &= ~(ulong)(1);          // flip it so that we do not round up
        }
      }

      answer.mantissa += (answer.mantissa & 1); // round up
      answer.mantissa >>= 1;
      if (answer.mantissa >= ((ulong)(2) << FloatBinaryConstants.mantissa_explicit_bits))
      {
        answer.mantissa = ((ulong)(1) << FloatBinaryConstants.mantissa_explicit_bits);
        answer.power2++; // undo previous addition
      }

      answer.mantissa &= ~((ulong)(1) << FloatBinaryConstants.mantissa_explicit_bits);
      if (answer.power2 >= FloatBinaryConstants.infinite_power)
      { // infinity
        answer.power2 = FloatBinaryConstants.infinite_power;
        answer.mantissa = 0;
      }
      return answer;
    }

    /// <summary>
    /// Fast-float algorithm - fallback using decimal info
    /// </summary>
    /// <param name="d">Decimal info structure</param>
    /// <returns>Adjusted mantissa</returns>
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
        answer.power2 = FloatBinaryConstants.infinite_power;
        answer.mantissa = 0;
        return answer;
      }
      const int max_shift = 60;
      const uint num_powers = 19;

      int exp2 = 0;
      while (d.decimal_point > 0)
      {
        uint n = (uint)(d.decimal_point);
        int shift = (n < num_powers) ? CalculationConstants.get_powers(n) : max_shift;

        d.decimal_right_shift(shift);
        if (d.decimal_point < -CalculationConstants.decimal_point_range)
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
          shift = (n < num_powers) ? CalculationConstants.get_powers(n) : max_shift;
        }

        d.decimal_left_shift(shift);

        if (d.decimal_point > CalculationConstants.decimal_point_range)
        {
          // we want to get infinity:
          answer.power2 = FloatBinaryConstants.infinite_power;
          answer.mantissa = 0;
          return answer;
        }
        exp2 -= (int)(shift);
      }
      // We are now in the range [1/2 ... 1] but the binary format uses [1 ... 2].
      exp2--;

      int min_exp = FloatBinaryConstants.minimum_exponent;

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
      if ((exp2 - min_exp) >= FloatBinaryConstants.infinite_power)
      {
        answer.power2 = FloatBinaryConstants.infinite_power;
        answer.mantissa = 0;
        return answer;
      }

      int mantissa_size_in_bits = FloatBinaryConstants.mantissa_explicit_bits + 1;
      d.decimal_left_shift((int)mantissa_size_in_bits);

      ulong mantissa = d.round();
      // It is possible that we have an overflow, in which case we need
      // to shift back.
      if (mantissa >= ((ulong)(1) << mantissa_size_in_bits))
      {
        d.decimal_right_shift(1);
        exp2 += 1;
        mantissa = d.round();
        if ((exp2 - min_exp) >= FloatBinaryConstants.infinite_power)
        {
          answer.power2 = FloatBinaryConstants.infinite_power;
          answer.mantissa = 0;
          return answer;
        }
      }
      answer.power2 = exp2 - min_exp;
      if (mantissa < ((ulong)(1) << FloatBinaryConstants.mantissa_explicit_bits)) { answer.power2--; }
      answer.mantissa = mantissa & (((ulong)(1) << FloatBinaryConstants.mantissa_explicit_bits) - 1);

      return answer;
    }

    // UTF-16 inputs
    internal static unsafe AdjustedMantissa ParseLongMantissa(char* first, char* last, char decimal_separator)
    {
      DecimalInfo d = DecimalInfo.parse_decimal(first, last, decimal_separator);
      return ComputeFloat(d);
    }

    // UTF-8/ASCII inputs
    internal static unsafe AdjustedMantissa ParseLongMantissa(byte* first, byte* last, byte decimal_separator)
    {
      DecimalInfo d = DecimalInfo.parse_decimal(first, last, decimal_separator);
      return ComputeFloat(d);
    }

    
  }



}
