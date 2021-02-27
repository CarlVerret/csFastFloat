﻿using csFastFloat.Enums;
using System.Runtime.CompilerServices;

namespace csFastFloat.Structures
{
  public unsafe struct ParsedNumberString
  {
    internal long exponent;
    internal ulong mantissa;
  
    internal int characters_consumed;
    internal bool negative;
    internal bool valid;
    internal bool too_many_digits;

    // UTF-16 inputs.
    unsafe static internal ParsedNumberString ParseNumberString(char* p, char* pend, chars_format expectedFormat = chars_format.is_general, char decimal_separator = '.')
    {
      ParsedNumberString answer = new ParsedNumberString();

      answer.valid = false;
      answer.too_many_digits = false;
      char* pstart = p;
      answer.negative = (*p == '-');
      if ((*p == '-') || (*p == '+'))
      {
        ++p;
        if (p == pend)
        {
          return answer;
        }
        if (!Utils.is_integer(*p, out uint _) && (*p != decimal_separator)) // culture info ?
        { // a  sign must be followed by an integer or the dot
          return answer;
        }
      }
      char* start_digits = p;

      ulong i = 0; // an unsigned int avoids signed overflows (which are bad)

      while ((p != pend) && Utils.is_integer(*p, out uint cMinus0))
      {
        // a multiplication by 10 is cheaper than an arbitrary integer
        // multiplication
        i = 10 * i + (ulong)cMinus0; // might overflow, we will handle the overflow later
        ++p;
      }
      char* end_of_integer_part = p;
      long digit_count = (long)(end_of_integer_part - start_digits);
      long exponent = 0;
      if ((p != pend) && (*p == decimal_separator))
      {
        ++p;
        while ((p != pend) && Utils.is_integer(*p, out uint cMinus0))
        {
          byte digit = (byte)cMinus0;
          ++p;
          i = i * 10 + digit; // in rare cases, this will overflow, but that's ok
        }
        exponent = end_of_integer_part + 1 - p;
        digit_count -= exponent;
      }
      // we must have encountered at least one integer!
      if (digit_count == 0)
      {
        return answer;
      }
      long exp_number = 0;            // explicit exponential part
      if (expectedFormat.HasFlag(chars_format.is_scientific) && (p != pend) && (('e' == *p) || ('E' == *p)))
      {
        char* location_of_e = p;
        ++p;
        bool neg_exp = false;
        if ((p != pend) && ('-' == *p))
        {
          neg_exp = true;
          ++p;
        }
        else if ((p != pend) && ('+' == *p))
        {
          ++p;
        }
        if ((p == pend) || !Utils.is_integer(*p, out uint _))
        {
          if (expectedFormat != chars_format.is_fixed)
          {
            // We are in error.
            return answer;
          }
          // Otherwise, we will be ignoring the 'e'.
          p = location_of_e;
        }
        else
        {
          while ((p != pend) && Utils.is_integer(*p, out uint cMinus0))
          {
            byte digit = (byte)cMinus0;
            if (exp_number < 0x10000)
            {
              exp_number = 10 * exp_number + digit;
            }
            ++p;
          }
          if (neg_exp) { exp_number = -exp_number; }
          exponent += exp_number;
        }
      }
      else
      {
        // If it scientific and not fixed, we have to bail out.
        if ((expectedFormat.HasFlag(chars_format.is_scientific)) && !(expectedFormat.HasFlag(chars_format.is_fixed))) { return answer; }
      }
      answer.valid = true;
      answer.characters_consumed = (int) (p - pstart);

      // If we frequently had to deal with long strings of digits,
      // we could extend our code by using a 128-bit integer instead
      // of a 64-bit integer. However, this is uncommon.
      //
      // We can deal with up to 19 digits.
      if (digit_count > 19)
      { // this is uncommon
        // It is possible that the integer had an overflow.
        // We have to handle the case where we have 0.0000somenumber.
        // We need to be mindful of the case where we only have zeroes...
        // E.g., 0.000000000...000.
        char* start = start_digits;
        while ((start != pend) && (*start == '0' || *start == decimal_separator))
        {
          if (*start == '0') { digit_count--; }
          start++;
        }
        if (digit_count > 19)
        {
          answer.too_many_digits = true;
          // Let us start again, this time, avoiding overflows.
          i = 0;
          p = start_digits;
          const ulong minimal_nineteen_digit_integer = 1000000000000000000;
          while ((i < minimal_nineteen_digit_integer) && (p != pend) && Utils.is_integer(*p, out uint cMinus0))
          {
            i = i * 10 + (ulong)cMinus0;
            ++p;
          }
          if (i >= minimal_nineteen_digit_integer)
          { // We have a big integers
            exponent = end_of_integer_part - p + exp_number;
          }
          else
          { // We have a value with a fractional component.
            p++; // skip the '.'
            char* first_after_period = p;
            while ((i < minimal_nineteen_digit_integer) && (p != pend) && Utils.is_integer(*p, out uint cMinus0))
            {
              i = i * 10 + (ulong)cMinus0;
              ++p;
            }
            exponent = first_after_period - p + exp_number;
          }
          // We have now corrected both exponent and i, to a truncated value
        }
      }
      answer.exponent = exponent;
      answer.mantissa = i;
      return answer;
    }

    // UTF-8 / ASCII inputs.
    unsafe static internal ParsedNumberString ParseNumberString(byte* p, byte* pend, chars_format expectedFormat = chars_format.is_general, byte decimal_separator = (byte)'.')
    {
      ParsedNumberString answer = new ParsedNumberString();

      answer.valid = false;
      answer.too_many_digits = false;
      byte* pstart = p;
      answer.negative = (*p == '-');
      if ((*p == '-') || (*p == '+'))
      {
        ++p;
        if (p == pend)
        {
          return answer;
        }
        if (!Utils.is_integer(*p, out uint digit) && (*p != decimal_separator))
        { // a  sign must be followed by an integer or the dot
          return answer;
        }
      }
      byte* start_digits = p;

      ulong i = 0; // an unsigned int avoids signed overflows (which are bad)

      while ((p != pend) && Utils.is_integer(*p, out uint digit))
      {
        // a multiplication by 10 is cheaper than an arbitrary integer
        // multiplication
        i = 10 * i + digit; // might overflow, we will handle the overflow later
        ++p;
      }
      byte* end_of_integer_part = p;
      long digit_count = (long)(end_of_integer_part - start_digits);
      long exponent = 0;
      if ((p != pend) && (*p == decimal_separator))
      {
        ++p;
        if ((p + 8 <= pend) && Utils.is_made_of_eight_digits_fast(p))
        {
          i = i * 100000000 + Utils.parse_eight_digits_unrolled(p);
          p += 8;
          if ((p + 8 <= pend) && Utils.is_made_of_eight_digits_fast(p)) {
            i = i * 100000000 + Utils.parse_eight_digits_unrolled(p);
            p += 8;
          }
        }
        while ((p != pend) && Utils.is_integer(*p, out uint digit))
        {
          ++p;
          i = i * 10 + digit; // in rare cases, this will overflow, but that's ok
        }
        exponent = end_of_integer_part + 1 - p;
        digit_count -= exponent;
      }
      // we must have encountered at least one integer!
      if (digit_count == 0)
      {
        return answer;
      }
      long exp_number = 0;            // explicit exponential part
      if (expectedFormat.HasFlag(chars_format.is_scientific) && (p != pend) && (('e' == *p) || ('E' == *p)))
      {
        byte* location_of_e = p;
        ++p;
        bool neg_exp = false;
        if ((p != pend) && ('-' == *p))
        {
          neg_exp = true;
          ++p;
        }
        else if ((p != pend) && ('+' == *p))
        {
          ++p;
        }
        if ((p == pend) || !Utils.is_integer(*p, out uint digit))
        {
          if (expectedFormat != chars_format.is_fixed)
          {
            // We are in error.
            return answer;
          }
          // Otherwise, we will be ignoring the 'e'.
          p = location_of_e;
        }
        else
        {
          while ((p != pend) && Utils.is_integer(*p, out uint cdigit))
          {
            if (exp_number < 0x10000)
            {
              exp_number = 10 * exp_number + cdigit;
            }
            ++p;
          }
          if (neg_exp) { exp_number = -exp_number; }
          exponent += exp_number;
        }
      }
      else
      {
        // If it scientific and not fixed, we have to bail out.
        if ((expectedFormat.HasFlag(chars_format.is_scientific)) && !(expectedFormat.HasFlag(chars_format.is_fixed))) { return answer; }
      }
      answer.valid = true;
      answer.characters_consumed = (int) (p - pstart);

      // If we frequently had to deal with long strings of digits,
      // we could extend our code by using a 128-bit integer instead
      // of a 64-bit integer. However, this is uncommon.
      //
      // We can deal with up to 19 digits.
      if (digit_count > 19)
      { // this is uncommon
        // It is possible that the integer had an overflow.
        // We have to handle the case where we have 0.0000somenumber.
        // We need to be mindful of the case where we only have zeroes...
        // E.g., 0.000000000...000.
        byte* start = start_digits;
        while ((start != pend) && (*start == '0' || *start == decimal_separator))
        {
          if (*start == '0') { digit_count--; }
          start++;
        }
        if (digit_count > 19)
        {
          answer.too_many_digits = true;
          // Let us start again, this time, avoiding overflows.
          i = 0;
          p = start_digits;
          const ulong minimal_nineteen_digit_integer = 1000000000000000000;
          while ((i < minimal_nineteen_digit_integer) && (p != pend) && Utils.is_integer(*p, out uint digit))
          {
            i = i * 10 + digit;
            ++p;
          }
          if (i >= minimal_nineteen_digit_integer)
          { // We have a big integers
            exponent = end_of_integer_part - p + exp_number;
          }
          else
          { // We have a value with a fractional component.
            p++; // skip the '.'
            byte* first_after_period = p;
            while ((i < minimal_nineteen_digit_integer) && (p != pend) && Utils.is_integer(*p, out uint digit))
            {
              i = i * 10 + digit;
              ++p;
            }
            exponent = first_after_period - p + exp_number;
          }
          // We have now corrected both exponent and i, to a truncated value
        }
      }
      answer.exponent = exponent;
      answer.mantissa = i;
      return answer;
    }

  };
}