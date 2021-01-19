using System;
using System.Globalization;
using System.Numerics;

namespace MonFloatParser
{
  public static class DoubleParser
  {
    /// <summary>
    /// Parse a string representation of a number
    /// </summary>
    /// <param name="p">String reprensentation</param>
    /// <returns>double value</returns>
    public static double? parse_number(string p)
    {
      double? res;

      int pos = 0;
      int first_after_period, start_digits;
      long exponent = 0;
      int digit_count;

      // an unsigned int avoids signed overflows (which are bad)
      ulong i;
      bool negative = false;

      // parse the number at p
      // return the null on error

      if (p.StartsWith("-"))
      {
        negative = true; pos++;
      }
      // a negative sign must be followed by an integer
      start_digits = pos;

      if (!Utils.is_integer(p, pos)) return null;

      // case - starts with a zero
      if (p[pos] == '0')
      {
        i = 0;
        pos += 1;
        if (Utils.is_integer(p, pos))
          return null;
      }
      else
      {
        if (!Utils.is_integer(p, pos)) return null;
        i = (ulong)Utils.as_digit(p, pos);
        pos++;
      }
      while (Utils.is_integer(p, pos))
      {
        var digit = Utils.as_digit(p, pos);
        // a multiplication by 10 is cheaper than an arbitrary integer
        // multiplication
        i = 10 * i + (ulong)digit; // might overflow, we will handle the overflow later
        pos++;
      }

      // case '.'
      if (Utils.as_char(p, pos) == '.')
      {
        pos++;
        first_after_period = pos;
        if (!Utils.is_integer(p, pos))
          return null;

        var digit = (ulong)Utils.as_digit(p, pos);
        i = i * 10 + digit; // might overflow + multiplication by 10 is likely
                            // cheaper than arbitrary mult.
                            // we will handle the overflow later
        pos++;

        while (Utils.is_integer(p, pos))
        {
          digit = (ulong)Utils.as_digit(p, pos);
          i = i * 10 + digit; // might overflow + multiplication by 10 is likely
                              // cheaper than arbitrary mult.
                              // we will handle the overflow later
          pos++;
        }
        exponent = (first_after_period - pos);
      }

      digit_count = pos - start_digits - 1; // used later to guard against overflows

      // case 'e' / 'E'
      if ("Ee".Contains(Utils.as_char(p, pos)))
      {
        pos++;
        bool neg_exp = false;
        if ("+-".Contains(Utils.as_char(p, pos)))
        {
          neg_exp = Utils.as_char(p, pos) == '-';
          pos++;
        }
        // at least one integer after + /- ...
        if (!Utils.is_integer(p, pos)) return null;

        long exp_number = Utils.as_digit(p, pos);
        pos++;

        if (Utils.is_integer(p, pos))
        {
          exp_number = 10 * exp_number + Utils.as_digit(p, pos); // might overflow + multiplication by 10 is likely
                                                                 // cheaper than arbitrary mult.
                                                                 // we will handle the overflow later
          pos++;
        }
        if (Utils.is_integer(p, pos))
        {
          exp_number = 10 * exp_number + Utils.as_digit(p, pos);
          pos++;
        }
        while (Utils.is_integer(p, pos))
        {
          if (exp_number < 0x100000000)
          { // we need to check for overflows
            exp_number = 10 * exp_number + Utils.as_digit(p, pos);
          }
          pos++;
        }

        if (neg_exp)
        {
          exponent -= exp_number;
        }
        else
        {
          exponent += exp_number;
        }
      }

      // If we frequently had to deal with long strings of digits,
      // we could extend our code by using a 128-bit integer instead
      // of a 64-bit integer. However, this is uncommon.
      if (digit_count >= 19)
      { // this is uncommon
        // It is possible that the integer had an overflow.
        // We have to handle the case where we have 0.0000somenumber.
        int start = start_digits;
        while ("0.".Contains(Utils.as_char(p, start)))
        {
          start++;
        }
        // we over-decrement by one when there is a decimal separator
        digit_count -= (start - start_digits);
        if (digit_count >= 19)
        {
          // Chances are good that we had an overflow!
          // We start anew.
          // This will happen in the following examples:
          // 10000000000000000000000000000000000000000000e+308
          // 3.1415926535897932384626433832795028841971693993751
          //
          return parse_float_strtod(p);
        }
      }
      if ((int)exponent < Constants.FASTFLOAT_SMALLEST_POWER || (int)exponent > Constants.FASTFLOAT_LARGEST_POWER)
      {
        // this is almost never going to get called!!!
        // exponent could be as low as 325
        return parse_float_strtod(p);
        //return parse_float_strtod(pinit, outDouble);
      }
      // from this point forward, exponent between FASTFLOAT_SMALLEST_POWER and FASTFLOAT_LARGEST_POWER

      res = compute_float_64((int)exponent, i, negative, out bool success);
      if (!success)
      {
        return parse_float_strtod(p);
      }
      // Todo
      return res;
    }

    /// <summary>
    /// compute a 64 bit float value
    /// </summary>
    /// <param name="power"></param>
    /// <param name="i"></param>
    /// <param name="negative"> bool : True indicates a negative number</param>
    /// <param name="success"> bool : true indicates sucesfull calculation</param>
    /// <returns></returns>
    public unsafe static double? compute_float_64(int power, ulong i, bool negative, out bool success)
    {
      double d;

      // we start with a fast path
      // It was described in
      // Clinger WD. How to read floating point numbers accurately.
      // ACM SIGPLAN Notices. 1990

      //#if FLT_EVAL_METHOD != 1 && FLT_EVAL_METHOD != 0
      //  // we do not trust the divisor
      //		if (0 <= power && power <= 22 && i <= 9007199254740991) {
      //#else
      if (-22 <= power && power <= 22 && i <= 9007199254740991)

      //#endif
      {
        // convert the integer into a double. This is lossless since
        // 0 <= i <= 2^53 - 1.
        d = (double)i;
        //
        // The general idea is as follows.
        // If 0 <= s < 2^53 and if 10^0 <= p <= 10^22 then
        // 1) Both s and p can be represented exactly as 64-bit floating-point
        // values
        // (binary64).
        // 2) Because s and p can be represented exactly as floating-point values,
        // then s * p
        // and s / p will produce correctly rounded values.
        //
        if (power < 0)
        {
          d /= Constants.power_of_ten[-power];
        }
        else
        {
          d *= Constants.power_of_ten[power];
        }
        success = true;
        return (negative ? -d : d);
      }
      // When 22 < power && power <  22 + 16, we could
      // hope for another, secondary fast path.  It wa
      // described by David M. Gay in  "Correctly rounded
      // binary-decimal and decimal-binary conversions." (1990)
      // If you need to compute i * 10^(22 + x) for x < 16,
      // first compute i * 10^x, if you know that result is exact
      // (e.g., when i * 10^x < 2^53),
      // then you can still proceed and do (i * 10^x) * 10^22.
      // Is this worth your time?
      // You need  22 < power *and* power <  22 + 16 *and* (i * 10^(x-22) < 2^53)
      // for this second fast path to work.
      // If you you have 22 < power *and* power <  22 + 16, and then you
      // optimistically compute "i * 10^(x-22)", there is still a chance that you
      // have wasted your time if i * 10^(x-22) >= 2^53. It makes the use cases of
      // this optimization maybe less common than we would like. Source:
      // http://www.exploringbinary.com/fast-path-decimal-to-floating-point-conversion/
      // also used in RapidJSON: https://rapidjson.org/strtod_8h_source.html

      // The fast path has now failed, so we are failing back on the slower path.

      // In the slow path, we need to adjust i so that it is > 1<<63 which is always
      // possible, except if i == 0, so we handle i == 0 separately.
      if (i == 0)
      {
        success = true;
        return 0.0;
      }

      // We are going to need to do some 64-bit arithmetic to get a more precise product.
      // We use a table lookup approach.
      // It is safe because
      // power >= FASTFLOAT_SMALLEST_POWER
      // and power <= FASTFLOAT_LARGEST_POWER
      // We recover the mantissa of the power, it has a leading 1. It is always
      // rounded down.
      ulong factor_mantissa = Constants.mantissa_64[power - Constants.FASTFLOAT_SMALLEST_POWER];

      // The exponent is 1024 + 63 + power
      //     + floor(log(5**power)/log(2)).
      // The 1024 comes from the ieee64 standard.
      // The 63 comes from the fact that we use a 64-bit word.
      //
      // Computing floor(log(5**power)/log(2)) could be
      // slow. Instead we use a fast function.
      //
      // For power in (-400,350), we have that
      // (((152170 + 65536) * power ) >> 16);
      // is equal to
      //  floor(log(5**power)/log(2)) + power when power >= 0
      // and it is equal to
      //  ceil(log(5**-power)/log(2)) + power when power < 0
      //
      //
      // The 65536 is (1<<16) and corresponds to
      // (65536 * power) >> 16 ---> power
      //
      // ((152170 * power ) >> 16) is equal to
      // floor(log(5**power)/log(2))
      //
      // Note that this is not magic: 152170/(1<<16) is
      // approximatively equal to log(5)/log(2).
      // The 1<<16 value is a power of two; we could use a
      // larger power of 2 if we wanted to.
      //
      long exponent = (((152170 + 65536) * power) >> 16) + 1024 + 63;
      // We want the most significant bit of i to be 1. Shift if needed.
      int lz = BitOperations.LeadingZeroCount(i);
      i <<= lz;
      // We want the most significant 64 bits of the product. We know
      // this will be non-zero because the most significant bit of i is
      // 1.
      value128 product = Utils.full_multiplication(i, factor_mantissa);
      ulong lower = product.low;
      ulong upper = product.high;
      // We know that upper has at most one leading zero because
      // both i and  factor_mantissa have a leading one. This means
      // that the result is at least as large as ((1<<63)*(1<<63))/(1<<64).

      // As long as the first 9 bits of "upper" are not "1", then we
      // know that we have an exact computed value for the leading
      // 55 bits because any imprecision would play out as a +1, in
      // the worst case.
      // Having 55 bits is necessary because
      // we need 53 bits for the mantissa but we have to have one rounding bit and
      // we can waste a bit if the most significant bit of the product is zero.
      // We expect this next branch to be rarely taken (say 1% of the time).
      // When (upper & 0x1FF) == 0x1FF, it can be common for
      // lower + i < lower to be true (proba. much higher than 1%).

      if (((upper & 0x1FF) == 0x1FF) && (lower + i < lower))
      {
        ulong factor_mantissa_low = Constants.mantissa_128[power - Constants.FASTFLOAT_SMALLEST_POWER];
        // next, we compute the 64-bit x 128-bit multiplication, getting a 192-bit
        // result (three 64-bit values)
        product = Utils.full_multiplication(i, factor_mantissa_low);
        ulong product_low = product.low;
        ulong product_middle2 = product.high;
        ulong product_middle1 = lower;
        ulong product_high = upper;
        ulong product_middle = product_middle1 + product_middle2;
        if (product_middle < product_middle1)
        {
          product_high++; // overflow carry
        }
        // we want to check whether mantissa *i + i would affect our result
        // This does happen, e.g. with 7.3177701707893310e+15
        if (((product_middle + 1 == 0) && ((product_high & 0x1FF) == 0x1FF) &&
          (product_low + i < product_low)))
        { // let us be prudent and bail out.
          success = false;
          return 0x00;
        }
        upper = product_high;
        lower = product_middle;
      }
      // The final mantissa should be 53 bits with a leading 1.
      // We shift it so that it occupies 54 bits with a leading 1.
      ///////
      ulong upperbit = upper >> 63;
      ulong mantissa = upper >> (int)(upperbit + 9);
      lz += (int)(1 ^ upperbit);
      // Here we have mantissa < (1<<54).

      // We have to round to even. The "to even" part
      // is only a problem when we are right in between two floats
      // which we guard against.
      // If we have lots of trailing zeros, we may fall right between two
      // floating-point values.
      if ((lower == 0) && ((upper & 0x1FF) == 0) && ((mantissa & 3) == 1))
      {
        // if mantissa & 1 == 1 we might need to round up.
        //
        // Scenarios:
        // 1. We are not in the middle. Then we should round up.
        //
        // 2. We are right in the middle. Whether we round up depends
        // on the last significant bit: if it is "one" then we round
        // up (round to even) otherwise, we do not.
        //
        // So if the last significant bit is 1, we can safely round up.
        // Hence we only need to bail out if (mantissa & 3) == 1.
        // Otherwise we may need more accuracy or analysis to determine whether
        // we are exactly between two floating-point numbers.
        // It can be triggered with 1e23.
        // Note: because the factor_mantissa and factor_mantissa_low are
        // almost always rounded down (except for small positive powers),
        // almost always should round up.
        success = false;
        return 0.0;
      }
      mantissa += mantissa & 1;
      mantissa >>= 1;
      // Here we have mantissa < (1<<53), unless there was an overflow
      if (mantissa >= ((ulong)1 << 53))
      {
        //////////
        // This will happen when parsing values such as 7.2057594037927933e+16
        ////////
        mantissa = ((ulong)1 << 52);
        lz--; // undo previous addition
      }
      mantissa &= ~((ulong)1 << 52);
      ulong real_exponent = (ulong)(exponent - lz);
      // we have to check that real_exponent is in range, otherwise we bail out
      if ((real_exponent < 1) || (real_exponent > 2046))
      {
        success = false;
        return 0.0;
      }
      mantissa |= real_exponent << 52;
      // Todo
      mantissa |= ((ulong)(negative ? 1 : 0) << 63);

      Buffer.MemoryCopy(&mantissa, &d, sizeof(double), sizeof(double));
      success = true;
      return d;
    }

    private static double parse_float_strtod(string p) => Double.Parse(p, CultureInfo.InvariantCulture);
  }
}