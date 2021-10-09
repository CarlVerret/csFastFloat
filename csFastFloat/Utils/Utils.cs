using csFastFloat.Constants;
using csFastFloat.Structures;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

[assembly: InternalsVisibleTo("TestcsFastFloat")]
[assembly: InternalsVisibleTo("vTuneBench")]

namespace csFastFloat
{

  internal static class Utils
  {
#if !HAS_BITOPERATIONS
    private static ReadOnlySpan<byte> Log2DeBruijn => new byte[]
    {
      00, 09, 01, 10, 13, 21, 02, 29,
      11, 14, 16, 18, 22, 25, 03, 30,
      08, 12, 20, 28, 15, 17, 24, 07,
      19, 27, 23, 06, 26, 05, 04, 31
    };
#endif
    
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
    unsafe internal static uint parse_eight_digits_unrolled(byte* chars)
    {
      ulong val = Unsafe.ReadUnaligned<ulong>(chars);
      return parse_eight_digits_unrolled(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static bool is_made_of_eight_digits_fast(ulong val)
    {
      // We only enable paths depending on this function on little endian
      // platforms (it happens to be effectively nearly everywhere).
      return BitConverter.IsLittleEndian && (((val & 0xF0F0F0F0F0F0F0F0) |
               (((val + 0x0606060606060606) & 0xF0F0F0F0F0F0F0F0) >> 4)) ==
              0x3333333333333333);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static bool is_made_of_eight_digits_fast(byte* chars)
    {
      ulong val = Unsafe.ReadUnaligned<ulong>(chars);
      return is_made_of_eight_digits_fast(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_integer(char c, out uint cMinus0)
    {
      uint cc = (uint)(c - '0');
      bool res = cc <= '9' - '0';
      cMinus0 = cc;
      return res;
    }

#if NET5_0

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static bool is_made_of_eight_digits_fast_simd(char* chars)
    {

      
      // We only enable paths depending on this function on little endian
      // platforms (it happens to be effectively nearly everywhere).

      Vector128<short> ascii0 = Vector128.Create((short)47);
      Vector128<short> after_ascii9 = Vector128.Create((short)58);


      Vector128<short> raw = Sse41.LoadDquVector128((short*)chars);

      var a = Sse41.CompareGreaterThan(raw, ascii0);
      var b = Sse41.CompareLessThan(raw, after_ascii9);
      var c = Sse41.Subtract(a, b); ;

      //was: return Sse2.Equals(c, Vector128<short>.Zero);
      var res = (Sse41.TestZ(c, c));
      return res;

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static uint parse_eight_digits_simd(char* start)
    {



        Vector128<ushort> raw = Sse41.LoadDquVector128((ushort*)start);
        Vector128<ushort> mask0 = Vector128.Create((ushort)48);
        raw = Sse2.SubtractSaturate(raw, mask0);
        Vector128<short> mul0 = Vector128.Create(10, 1, 10, 1, 10, 1, 10, 1);
        Vector128<int> res = Sse41.MultiplyAddAdjacent(raw.AsInt16(), mul0);
        Vector128<int> mul1 = Vector128.Create(1000000, 10000, 100, 1);
        res = Sse41.MultiplyLow(res, mul1);
        Vector128<int> shuf = Sse41.Shuffle(res, 0x1b); // 0 1 2 3 => 3 2 1 0
        res = Sse41.Add(shuf, res);
        shuf = Sse41.Shuffle(res, 0x41); // 0 1 2 3 => 1 0 3 2
        res = Sse41.Add(shuf, res);

        return (uint)res.GetElement(0);
      
    }




    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static bool eval_parse_eight_digits_simd(char* start,  char* end, out uint value )
    {

      value = 0;

      Vector128<short> raw = Sse41.LoadDquVector128((short*)start);

      Vector128<short> ascii0 = Vector128.Create((short)47);
      Vector128<short> after_ascii9 = Vector128.Create((short)58);

      var a = Sse41.CompareGreaterThan(raw, ascii0);
      var b = Sse41.CompareLessThan(raw, after_ascii9);
      var c = Sse41.Subtract(a, b);

      if (!Sse41.TestZ(c, c))
        return false;
   
      Vector128<short> mask0 = Vector128.Create((short)48); // adapter à la longueur
      raw = Sse41.SubtractSaturate(raw, mask0);
      Vector128<short> mul0 = Vector128.Create(10, 1, 10, 1, 10, 1, 10, 1);
      Vector128<int> res = Sse41.MultiplyAddAdjacent(raw.AsInt16(), mul0);
      Vector128<int> mul1 = Vector128.Create(1000000, 10000, 100, 1);
      res = Sse41.MultiplyLow(res, mul1);
      Vector128<int> shuf = Sse41.Shuffle(res, 0x1b); // 0 1 2 3 => 3 2 1 0
      res = Sse41.Add(shuf, res);
      shuf = Sse41.Shuffle(res, 0x41); // 0 1 2 3 => 1 0 3 2
      res = Sse41.Add(shuf, res);

      value = (uint)res.GetElement(0);

      return true;

    }





#endif


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_integer(byte c, out uint cMinus0)
    {
      uint cc = (uint)(c - (byte)'0');
      bool res = cc <= '9' - '0';
      cMinus0 = cc;
      return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static value128 compute_product_approximation(int bitPrecision, long q, ulong w)
    {
      int index = 2 * (int)(q - CalculationConstants.smallest_power_of_five);
      // For small values of q, e.g., q in [0,27], the answer is always exact because
      // The line value128 firstproduct = full_multiplication(w, power_of_five_128[index]);
      // gives the exact answer.
      value128 firstproduct = FullMultiplication(w, CalculationConstants.get_power_of_five_128(index));
      //static_assert((bit_precision >= 0) && (bit_precision <= 64), " precision should  be in (0,64]");
      ulong precision_mask = (bitPrecision < 64) ? ((ulong)(0xFFFFFFFFFFFFFFFF) >> bitPrecision) : (ulong)(0xFFFFFFFFFFFFFFFF);
      if ((firstproduct.high & precision_mask) == precision_mask)
      { // could further guard with  (lower + w < lower)
        // regarding the second product, we only need secondproduct.high, but our expectation is that the compiler will optimize this extra work away if needed.
        value128 secondproduct = FullMultiplication(w, CalculationConstants.get_power_of_five_128(index + 1));
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

#if NET5_0

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static value128 FullMultiplication(ulong value1, ulong value2)
    {
      ulong hi = Math.BigMul(value1, value2, out ulong lo);
      return new value128(hi, lo);
    }

#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe value128 FullMultiplication(ulong value1, ulong value2)
    {
#if HAS_INTRINSICS
      if(System.Runtime.Intrinsics.X86.Bmi2.X64.IsSupported)
      {
            ulong lo;
            ulong hi = System.Runtime.Intrinsics.X86.Bmi2.X64.MultiplyNoFlags(value1, value2, &lo);
            return new value128(hi, lo);
      }
#endif
      return Emulate64x64to128( value1, value2);
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

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_ascii_space(char c)
    {
        // ROS for one byte types can be read directly from metadata avoiding the array allocation.
        ReadOnlySpan<bool> table = new bool[] {
            false, false, false, false, false, false, false, false, false, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, true};
        // Avoid bound checking.
        return (c >32) ? false : Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(table), (nint)c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_space(byte c)
    {
        // ROS for one byte types can be read directly from metadata avoiding the array allocation.
        ReadOnlySpan<bool> table = new bool[] {
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
        
        // Avoid bound checking.
        return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(table), (nint)c);
    }

    [ExcludeFromCodeCoverage]
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
    internal unsafe static bool strncasecmp(byte* input1, ReadOnlySpan<byte> input2, int length)
    {
      int running_diff = 0;

      for (int i = 0; i < length; i++)
      {
        running_diff = running_diff | (input1[i] ^ input2[i]);
      }
      return (running_diff == 0) || (running_diff == 32);
    }

    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ulong value)
    {
#if HAS_BITOPERATIONS
      return System.Numerics.BitOperations.LeadingZeroCount(value);
#else
      uint hi = (uint)(value >> 32);
 
      if (hi == 0)
        return 32 + Log2SoftwareFallback((uint)value);
 
      return Log2SoftwareFallback(hi);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      static int Log2SoftwareFallback(uint value)
      {
        if (value == 0)
          return 32;
        
        int n = 1;
        if (value >> 16 == 0) { n += 16; value <<= 16; }
        if (value >> 24 == 0) { n +=  8; value <<=  8; }
        if (value >> 28 == 0) { n +=  4; value <<=  4; }
        if (value >> 30 == 0) { n +=  2; value <<=  2; }
        n -= (int) (value >> 31);
        return n;
    
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe float Int32BitsToSingle(int value)
    {
#if HAS_BITOPERATIONS
      return BitConverter.Int32BitsToSingle(value);
#else
      return *((float*)&value);
#endif
    }

  }
}
