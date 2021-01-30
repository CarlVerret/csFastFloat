using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace csFastFloat.Structures
{
  internal sealed class DecimalInfo
  {
    internal uint num_digits = 0;
    internal int decimal_point = 0;
    internal bool negative = false;
    internal bool truncated = false;
    internal byte[] digits = new byte[Constants.max_digits];

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
      StringBuilder sb = new();

      sb.Append("0.");

      for (int i = 0; i < num_digits; i++)
      {
        sb.Append(digits[i]);
      }

      sb.Append($" * 10 ** {decimal_point} "); ;
      return sb.ToString();
    }

    internal void trim()
    {
      while ((num_digits > 0) && (digits[num_digits - 1] == 0))
      {
        num_digits--;
      }
    }

    private unsafe uint number_of_digits_decimal_left_shift(int shift)
    {
      shift &= 63;
      ushort[] number_of_digits_decimal_left_shift_table = {
    0x0000, 0x0800, 0x0801, 0x0803, 0x1006, 0x1009, 0x100D, 0x1812, 0x1817,
    0x181D, 0x2024, 0x202B, 0x2033, 0x203C, 0x2846, 0x2850, 0x285B, 0x3067,
    0x3073, 0x3080, 0x388E, 0x389C, 0x38AB, 0x38BB, 0x40CC, 0x40DD, 0x40EF,
    0x4902, 0x4915, 0x4929, 0x513E, 0x5153, 0x5169, 0x5180, 0x5998, 0x59B0,
    0x59C9, 0x61E3, 0x61FD, 0x6218, 0x6A34, 0x6A50, 0x6A6D, 0x6A8B, 0x72AA,
    0x72C9, 0x72E9, 0x7B0A, 0x7B2B, 0x7B4D, 0x8370, 0x8393, 0x83B7, 0x83DC,
    0x8C02, 0x8C28, 0x8C4F, 0x9477, 0x949F, 0x94C8, 0x9CF2, 0x051C, 0x051C,
    0x051C, 0x051C,
  };
      uint x_a = number_of_digits_decimal_left_shift_table[shift];
      uint x_b = number_of_digits_decimal_left_shift_table[shift + 1];
      uint num_new_digits = x_a >> 11;
      uint pow5_a = 0x7FF & x_a;
      uint pow5_b = 0x7FF & x_b;
      byte[] number_of_digits_decimal_left_shift_table_powers_of_5 = {
        5, 2, 5, 1, 2, 5, 6, 2, 5, 3, 1, 2, 5, 1, 5, 6, 2, 5, 7, 8, 1, 2, 5, 3,
        9, 0, 6, 2, 5, 1, 9, 5, 3, 1, 2, 5, 9, 7, 6, 5, 6, 2, 5, 4, 8, 8, 2, 8,
        1, 2, 5, 2, 4, 4, 1, 4, 0, 6, 2, 5, 1, 2, 2, 0, 7, 0, 3, 1, 2, 5, 6, 1,
        0, 3, 5, 1, 5, 6, 2, 5, 3, 0, 5, 1, 7, 5, 7, 8, 1, 2, 5, 1, 5, 2, 5, 8,
        7, 8, 9, 0, 6, 2, 5, 7, 6, 2, 9, 3, 9, 4, 5, 3, 1, 2, 5, 3, 8, 1, 4, 6,
        9, 7, 2, 6, 5, 6, 2, 5, 1, 9, 0, 7, 3, 4, 8, 6, 3, 2, 8, 1, 2, 5, 9, 5,
        3, 6, 7, 4, 3, 1, 6, 4, 0, 6, 2, 5, 4, 7, 6, 8, 3, 7, 1, 5, 8, 2, 0, 3,
        1, 2, 5, 2, 3, 8, 4, 1, 8, 5, 7, 9, 1, 0, 1, 5, 6, 2, 5, 1, 1, 9, 2, 0,
        9, 2, 8, 9, 5, 5, 0, 7, 8, 1, 2, 5, 5, 9, 6, 0, 4, 6, 4, 4, 7, 7, 5, 3,
        9, 0, 6, 2, 5, 2, 9, 8, 0, 2, 3, 2, 2, 3, 8, 7, 6, 9, 5, 3, 1, 2, 5, 1,
        4, 9, 0, 1, 1, 6, 1, 1, 9, 3, 8, 4, 7, 6, 5, 6, 2, 5, 7, 4, 5, 0, 5, 8,
        0, 5, 9, 6, 9, 2, 3, 8, 2, 8, 1, 2, 5, 3, 7, 2, 5, 2, 9, 0, 2, 9, 8, 4,
        6, 1, 9, 1, 4, 0, 6, 2, 5, 1, 8, 6, 2, 6, 4, 5, 1, 4, 9, 2, 3, 0, 9, 5,
        7, 0, 3, 1, 2, 5, 9, 3, 1, 3, 2, 2, 5, 7, 4, 6, 1, 5, 4, 7, 8, 5, 1, 5,
        6, 2, 5, 4, 6, 5, 6, 6, 1, 2, 8, 7, 3, 0, 7, 7, 3, 9, 2, 5, 7, 8, 1, 2,
        5, 2, 3, 2, 8, 3, 0, 6, 4, 3, 6, 5, 3, 8, 6, 9, 6, 2, 8, 9, 0, 6, 2, 5,
        1, 1, 6, 4, 1, 5, 3, 2, 1, 8, 2, 6, 9, 3, 4, 8, 1, 4, 4, 5, 3, 1, 2, 5,
        5, 8, 2, 0, 7, 6, 6, 0, 9, 1, 3, 4, 6, 7, 4, 0, 7, 2, 2, 6, 5, 6, 2, 5,
        2, 9, 1, 0, 3, 8, 3, 0, 4, 5, 6, 7, 3, 3, 7, 0, 3, 6, 1, 3, 2, 8, 1, 2,
        5, 1, 4, 5, 5, 1, 9, 1, 5, 2, 2, 8, 3, 6, 6, 8, 5, 1, 8, 0, 6, 6, 4, 0,
        6, 2, 5, 7, 2, 7, 5, 9, 5, 7, 6, 1, 4, 1, 8, 3, 4, 2, 5, 9, 0, 3, 3, 2,
        0, 3, 1, 2, 5, 3, 6, 3, 7, 9, 7, 8, 8, 0, 7, 0, 9, 1, 7, 1, 2, 9, 5, 1,
        6, 6, 0, 1, 5, 6, 2, 5, 1, 8, 1, 8, 9, 8, 9, 4, 0, 3, 5, 4, 5, 8, 5, 6,
        4, 7, 5, 8, 3, 0, 0, 7, 8, 1, 2, 5, 9, 0, 9, 4, 9, 4, 7, 0, 1, 7, 7, 2,
        9, 2, 8, 2, 3, 7, 9, 1, 5, 0, 3, 9, 0, 6, 2, 5, 4, 5, 4, 7, 4, 7, 3, 5,
        0, 8, 8, 6, 4, 6, 4, 1, 1, 8, 9, 5, 7, 5, 1, 9, 5, 3, 1, 2, 5, 2, 2, 7,
        3, 7, 3, 6, 7, 5, 4, 4, 3, 2, 3, 2, 0, 5, 9, 4, 7, 8, 7, 5, 9, 7, 6, 5,
        6, 2, 5, 1, 1, 3, 6, 8, 6, 8, 3, 7, 7, 2, 1, 6, 1, 6, 0, 2, 9, 7, 3, 9,
        3, 7, 9, 8, 8, 2, 8, 1, 2, 5, 5, 6, 8, 4, 3, 4, 1, 8, 8, 6, 0, 8, 0, 8,
        0, 1, 4, 8, 6, 9, 6, 8, 9, 9, 4, 1, 4, 0, 6, 2, 5, 2, 8, 4, 2, 1, 7, 0,
        9, 4, 3, 0, 4, 0, 4, 0, 0, 7, 4, 3, 4, 8, 4, 4, 9, 7, 0, 7, 0, 3, 1, 2,
        5, 1, 4, 2, 1, 0, 8, 5, 4, 7, 1, 5, 2, 0, 2, 0, 0, 3, 7, 1, 7, 4, 2, 2,
        4, 8, 5, 3, 5, 1, 5, 6, 2, 5, 7, 1, 0, 5, 4, 2, 7, 3, 5, 7, 6, 0, 1, 0,
        0, 1, 8, 5, 8, 7, 1, 1, 2, 4, 2, 6, 7, 5, 7, 8, 1, 2, 5, 3, 5, 5, 2, 7,
        1, 3, 6, 7, 8, 8, 0, 0, 5, 0, 0, 9, 2, 9, 3, 5, 5, 6, 2, 1, 3, 3, 7, 8,
        9, 0, 6, 2, 5, 1, 7, 7, 6, 3, 5, 6, 8, 3, 9, 4, 0, 0, 2, 5, 0, 4, 6, 4,
        6, 7, 7, 8, 1, 0, 6, 6, 8, 9, 4, 5, 3, 1, 2, 5, 8, 8, 8, 1, 7, 8, 4, 1,
        9, 7, 0, 0, 1, 2, 5, 2, 3, 2, 3, 3, 8, 9, 0, 5, 3, 3, 4, 4, 7, 2, 6, 5,
        6, 2, 5, 4, 4, 4, 0, 8, 9, 2, 0, 9, 8, 5, 0, 0, 6, 2, 6, 1, 6, 1, 6, 9,
        4, 5, 2, 6, 6, 7, 2, 3, 6, 3, 2, 8, 1, 2, 5, 2, 2, 2, 0, 4, 4, 6, 0, 4,
        9, 2, 5, 0, 3, 1, 3, 0, 8, 0, 8, 4, 7, 2, 6, 3, 3, 3, 6, 1, 8, 1, 6, 4,
        0, 6, 2, 5, 1, 1, 1, 0, 2, 2, 3, 0, 2, 4, 6, 2, 5, 1, 5, 6, 5, 4, 0, 4,
        2, 3, 6, 3, 1, 6, 6, 8, 0, 9, 0, 8, 2, 0, 3, 1, 2, 5, 5, 5, 5, 1, 1, 1,
        5, 1, 2, 3, 1, 2, 5, 7, 8, 2, 7, 0, 2, 1, 1, 8, 1, 5, 8, 3, 4, 0, 4, 5,
        4, 1, 0, 1, 5, 6, 2, 5, 2, 7, 7, 5, 5, 5, 7, 5, 6, 1, 5, 6, 2, 8, 9, 1,
        3, 5, 1, 0, 5, 9, 0, 7, 9, 1, 7, 0, 2, 2, 7, 0, 5, 0, 7, 8, 1, 2, 5, 1,
        3, 8, 7, 7, 7, 8, 7, 8, 0, 7, 8, 1, 4, 4, 5, 6, 7, 5, 5, 2, 9, 5, 3, 9,
        5, 8, 5, 1, 1, 3, 5, 2, 5, 3, 9, 0, 6, 2, 5, 6, 9, 3, 8, 8, 9, 3, 9, 0,
        3, 9, 0, 7, 2, 2, 8, 3, 7, 7, 6, 4, 7, 6, 9, 7, 9, 2, 5, 5, 6, 7, 6, 2,
        6, 9, 5, 3, 1, 2, 5, 3, 4, 6, 9, 4, 4, 6, 9, 5, 1, 9, 5, 3, 6, 1, 4, 1,
        8, 8, 8, 2, 3, 8, 4, 8, 9, 6, 2, 7, 8, 3, 8, 1, 3, 4, 7, 6, 5, 6, 2, 5,
        1, 7, 3, 4, 7, 2, 3, 4, 7, 5, 9, 7, 6, 8, 0, 7, 0, 9, 4, 4, 1, 1, 9, 2,
        4, 4, 8, 1, 3, 9, 1, 9, 0, 6, 7, 3, 8, 2, 8, 1, 2, 5, 8, 6, 7, 3, 6, 1,
        7, 3, 7, 9, 8, 8, 4, 0, 3, 5, 4, 7, 2, 0, 5, 9, 6, 2, 2, 4, 0, 6, 9, 5,
        9, 5, 3, 3, 6, 9, 1, 4, 0, 6, 2, 5,
  };
      // byte* pow5 = (byte*)number_of_digits_decimal_left_shift_table_powers_of_5[pow5_a];
      int i = 0;
      uint n = pow5_b - pow5_a;
      for (; i < n; i++)
      {
        if (i >= num_digits)
        {
          return num_new_digits - 1;
        }
        else if (digits[i] == number_of_digits_decimal_left_shift_table_powers_of_5[pow5_a + i])
        {
          continue;
        }
        else if (digits[i] < number_of_digits_decimal_left_shift_table_powers_of_5[pow5_a + i])
        {
          return num_new_digits - 1;
        }
        else
        {
          return num_new_digits;
        }
      }
      return num_new_digits;
    }

    internal ulong round()
    {
      if ((num_digits == 0) || (decimal_point < 0))
      {
        return 0;
      }
      else if (decimal_point > 18)
      {
        return ulong.MaxValue;
      }
      // at this point, we know that decimal_point >= 0
      uint dp = (uint)decimal_point;
      ulong n = 0;
      for (uint i = 0; i < dp; i++)
      {
        n = (10 * n) + (ulong)((i < num_digits) ? digits[i] : 0);
      }
      bool round_up = false;
      if (dp < num_digits)
      {
        round_up = digits[dp] >= 5; // normally, we round up
                                    // but we may need to round to even!
        if ((digits[dp] == 5) && (dp + 1 == num_digits))
        {
          round_up = truncated || ((dp > 0) && digits[dp - 1] % 2 != 0);
        }
      }
      if (round_up)
      {
        n++;
      }
      return n;
    }

    // computes h * 2^-shift
    internal void decimal_left_shift(int shift)
    {
      if (num_digits == 0)
      {
        return;
      }
      uint num_new_digits = number_of_digits_decimal_left_shift(shift);

      int read_index = (int)(num_digits - 1);
      uint write_index = num_digits - 1 + num_new_digits;
      ulong n = 0;

      while (read_index >= 0)
      {
        n += (ulong)(digits[read_index]) << shift;
        ulong quotient = n / 10;
        ulong remainder = n - (10 * quotient);
        if (write_index < Constants.max_digits)
        {
          digits[write_index] = (byte)(remainder);
        }
        else if (remainder > 0)
        {
          truncated = true;
        }
        n = quotient;
        write_index--;
        read_index--;
      }
      while (n > 0)
      {
        ulong quotient = n / 10;
        ulong remainder = n - (10 * quotient);
        if (write_index < Constants.max_digits)
        {
          digits[write_index] = (byte)(remainder);
        }
        else if (remainder > 0)
        {
          truncated = true;
        }
        n = quotient;
        write_index--;
      }
      num_digits += num_new_digits;
      if (num_digits > Constants.max_digits)
      {
        num_digits = Constants.max_digits;
      }
      decimal_point += (int)(num_new_digits);
      trim();
    }

    // computes h * 2^shift
    internal void decimal_right_shift(int shift)
    {
      uint read_index = 0;
      uint write_index = 0;

      ulong n = 0;

      while ((n >> shift) == 0)
      {
        if (read_index < num_digits)
        {
          n = (10 * n) + digits[read_index++];
        }
        else if (n == 0)
        {
          return;
        }
        else
        {
          while ((n >> shift) == 0)
          {
            n = 10 * n;
            read_index++;
          }
          break;
        }
      }
      decimal_point -= (int)(read_index - 1);
      if (decimal_point < -Constants.decimal_point_range)
      { // it is zero
        num_digits = 0;
        decimal_point = 0;
        negative = false;
        truncated = false;
        return;
      }
      ulong mask = ((ulong)(1) << shift) - 1;
      while (read_index < num_digits)
      {
        byte new_digit = (byte)(n >> shift);
        n = (10 * (n & mask)) + digits[read_index++];
        digits[write_index++] = new_digit;
      }
      while (n > 0)
      {
        byte new_digit = (byte)(n >> shift);
        n = 10 * (n & mask);
        if (write_index < Constants.max_digits)
        {
          digits[write_index++] = new_digit;
        }
        else if (new_digit > 0)
        {
          truncated = true;
        }
      }
      num_digits = write_index;
      trim();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static DecimalInfo parse_decimal(char* p, char* pend, char decimal_separator)
    {
      DecimalInfo answer = new DecimalInfo() { num_digits = 0, decimal_point = 0, truncated = false, negative = (*p == '-') };

      if ((*p == '-') || (*p == '+'))
      {
        ++p;
      }
      // skip leading zeroes
      while ((p != pend) && (*p == '0'))
      {
        ++p;
      }
      while ((p != pend) && Utils.is_integer(*p))
      {
        if (answer.num_digits < Constants.max_digits)
        {
          answer.digits[answer.num_digits] = (byte)(*p - '0');
        }
        answer.num_digits++;
        ++p;
      }
      if ((p != pend) && (*p == decimal_separator))
      {
        ++p;
        char* first_after_period = p;
        // if we have not yet encountered a zero, we have to skip it as well
        if (answer.num_digits == 0)
        {
          // skip zeros
          while ((p != pend) && (*p == '0'))
          {
            ++p;
          }
        }
        //#if FASTFLOAT_IS_BIG_ENDIAN == 0
        //        // We expect that this loop will often take the bulk of the running time
        //        // because when a value has lots of digits, these digits often
        //        while ((p + 8 <= pend) && (answer.num_digits + 8 < Constants.max_digits))
        //        {
        //          ulong val;
        //      ::memcpy(&val, p, sizeof(ulong));
        //          if (!is_made_of_eight_digits_fast(val)) { break; }
        //          // We have eight digits, process them in one go!
        //          val -= 0x3030303030303030;
        //      ::memcpy(answer.digits + answer.num_digits, &val, sizeof(ulong));
        //          answer.num_digits += 8;
        //          p += 8;
        //        }
        //#endif
        while ((p != pend) && Utils.is_integer(*p))
        {
          if (answer.num_digits < Constants.max_digits)
          {
            answer.digits[answer.num_digits] = (byte)(*p - '0');
          }
          answer.num_digits++;
          ++p;
        }
        answer.decimal_point = (int)(first_after_period - p);
      }
      // We want num_digits to be the number of significant digits, excluding
      // leading *and* trailing zeros! Otherwise the truncated flag later is
      // going to be misleading.
      if (answer.num_digits > 0)
      {
        // We potentially need the answer.num_digits > 0 guard because we
        // prune leading zeros. So with answer.num_digits > 0, we know that
        // we have at least one non-zero digit.
        char* preverse = p - 1;
        int trailing_zeros = 0;
        while ((*preverse == '0') || (*preverse == decimal_separator))
        {
          if (*preverse == '0') { trailing_zeros++; };
          --preverse;
        }
        answer.decimal_point += (int)(answer.num_digits);
        answer.num_digits -= (uint)(trailing_zeros);
      }
      if (answer.num_digits > Constants.max_digits)
      {
        answer.truncated = true;
        answer.num_digits = Constants.max_digits;
      }
      if ((p != pend) && (('e' == *p) || ('E' == *p)))
      {
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
        int exp_number = 0; // exponential part
        while ((p != pend) && Utils.is_integer(*p))
        {
          byte digit = (byte)(*p - '0');
          if (exp_number < 0x10000)
          {
            exp_number = 10 * exp_number + digit;
          }
          ++p;
        }
        answer.decimal_point += (neg_exp ? -exp_number : exp_number);
      }
      // In very rare cases, we may have fewer than 19 digits, we want to be able to reliably
      // assume that all digits up to max_digit_without_overflow have been initialized.
      for (uint i = answer.num_digits; i < Constants.max_digit_without_overflow; i++) { answer.digits[i] = 0; }

      return answer;
    }
  }
}