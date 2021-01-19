using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace cs_FastFloat
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
    // this gets compiled to (uint8_t)(c - '0') <= 9 on all decent compilers
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool is_integer(string s, int pos) => (
      pos < s.Length && s[pos] >= '0' && s[pos] <= '9');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int as_digit(string s, int pos) => s[pos] - '0';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char as_char(string s, int pos) => pos < s.Length ? s[pos] : '\0';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static value128 full_multiplication(ulong value1, ulong value2)
    {
      // Todo : prof !
      ulong lo;
      ulong hi = System.Runtime.Intrinsics.X86.Bmi2.X64.MultiplyNoFlags(value1, value2, &lo);
      return new value128(hi, lo);

      // when to fallback ?
      // return Emulate64x64to128(value1, value2);
    }

    private static value128 Emulate64x64to128(ulong x, ulong y)
    {
      ulong x0 = (uint)x, x1 = x >> 32;
      ulong y0 = (uint)y, y1 = y >> 32;
      ulong p11 = x1 * y1, p01 = x0 * y1;
      ulong p10 = x1 * y0, p00 = x0 * y0;

      ulong middle = p10 + (p00 >> 32) + (uint)p01;

      return new value128(h: p11 + (middle >> 32) + (p01 >> 32), l: (middle << 32) | (uint)p00);
    }
  }
}