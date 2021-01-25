using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{
  public struct value128
  {
    public ulong low;
    public ulong high;

    public value128(ulong h, ulong l) : this()
    {
      this.high = h;
      this.low = l;
    }
  }

  public static class Utils
  {
    // Next function can be micro-optimized, but compilers are entirely
    // able to optimize it well.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_integer(char c) => c >= '0' && c <= '9';

    // credit  @aqrit
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint parse_eight_digits_unrolled(ulong val)
    {
      const ulong mask = 0x000000FF000000FF;
      const ulong mul1 = 0x000F424000000064; // 100 + (1000000ULL << 32)
      const ulong mul2 = 0x0000271000000001; // 1 + (10000ULL << 32)
      val -= 0x3030303030303030;
      val = (val * 10) + (val >> 8); // val = (val * 2561) >> 8;
      val = (((val & mask) * mul1) + (((val >> 16) & mask) * mul2)) >> 32;
      return (uint)val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static uint parse_eight_digits_unrolled(char* chars)
    {
      ulong val;
      Buffer.MemoryCopy(chars, &val, sizeof(ulong), sizeof(ulong));
      return parse_eight_digits_unrolled(val);
    }

    // credit @aqrit
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static bool is_made_of_eight_digits_fast(char* chars)
    {
      return false;
      //ulong val;
      //Buffer.MemoryCopy(&chars, &val, 8, 8);

      //return !(((val & 0xF0F0F0F0F0F0F0F0) |
      //         (((val + 0x0606060606060606) & 0xF0F0F0F0F0F0F0F0) >> 4)) ==
      //        0x3333333333333333);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static value128 compute_product_approximation(int bitPrecision, long q, ulong w)
    {
      int index = 2 * (int)(q - Constants.smallest_power_of_five);
      // For small values of q, e.g., q in [0,27], the answer is always exact because
      // The line value128 firstproduct = full_multiplication(w, power_of_five_128[index]);
      // gives the exact answer.
      value128 firstproduct = FullMultiplication(w, Constants.power_of_five_128[index]);
      //static_assert((bit_precision >= 0) && (bit_precision <= 64), " precision should  be in (0,64]");
      ulong precision_mask = (bitPrecision < 64) ? ((ulong)(0xFFFFFFFFFFFFFFFF) >> bitPrecision) : (ulong)(0xFFFFFFFFFFFFFFFF);
      if ((firstproduct.high & precision_mask) == precision_mask)
      { // could further guard with  (lower + w < lower)
        // regarding the second product, we only need secondproduct.high, but our expectation is that the compiler will optimize this extra work away if needed.
        value128 secondproduct = FullMultiplication(w, Constants.power_of_five_128[index + 1]);
        firstproduct.low += secondproduct.high;
        if (secondproduct.high > firstproduct.low)
        {
          firstproduct.high++;
        }
      }
      return firstproduct;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int power(int q)
    {
      return (((152170 + 65536) * q) >> 16) + 63;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static value128 FullMultiplication(ulong value1, ulong value2)
    {
      ulong lo;
      //ulong hi = System.Runtime.Intrinsics.X86.Bmi2.X64.MultiplyNoFlags(value1, value2, &lo);
      ulong hi = Math.BigMul(value1, value2, out lo);

      return new value128(hi, lo);

      // when to fallback ?
      // return Emulate64x64to128(value1, value2);
    }

    internal static value128 Emulate64x64to128(ulong x, ulong y)
    {
      ulong x0 = (uint)x, x1 = x >> 32;
      ulong y0 = (uint)y, y1 = y >> 32;
      ulong p11 = x1 * y1, p01 = x0 * y1;
      ulong p10 = x1 * y0, p00 = x0 * y0;

      ulong middle = p10 + (p00 >> 32) + (uint)p01;

      return new value128(h: p11 + (middle >> 32) + (p01 >> 32), l: (middle << 32) | (uint)p00);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_space(byte c)
    {
      bool[] table = new bool[] {
      false, false, false, false, false, false, false, false, false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
      false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
      return table[c];
    }

    internal unsafe static bool strncasecmp(char* input1, string input2, int length)
    {
      fixed (char* p2 = input2)
      {
        return strncasecmp(input1, p2, length);
      }
    }

    internal unsafe static bool strncasecmp(char* input1, char* input2, int length)
    {
      int running_diff = 0;

      for (int i = 0; i < length; i++)
      {
        running_diff = running_diff | (input1[i] ^ input2[i]);
      }
      return (running_diff == 0) || (running_diff == 32);
    }
  }
}