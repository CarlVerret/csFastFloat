using csFastFloat.Constants;
using csFastFloat.Structures;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


#if HAS_INTRINSICS
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat
{

  internal static unsafe class Utils
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
    internal static uint parse_eight_digits_unrolled(byte* chars)
    {
      ulong val = Unsafe.ReadUnaligned<ulong>(chars);
      return parse_eight_digits_unrolled(val);
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_made_of_eight_digits_fast(byte* chars)
    {
      ulong val = Unsafe.ReadUnaligned<ulong>(chars);
      // We only enable paths depending on this function on little endian
      // platforms (it happens to be effectively nearly everywhere).
      // ref : https://lemire.me/blog/tag/swar/
      return BitConverter.IsLittleEndian && ((val & (val + 0x0606060606060606) & 0xF0F0F0F0F0F0F0F0) == 0x3030303030303030);

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool is_integer(char c, out uint cMinus0)
    {
      uint cc = (uint)(c - '0');
      bool res = cc <= '9' - '0';
      cMinus0 = cc;
      return res;
    }

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
        => (((152170 + 65536) * q) >> 16) + 63;

#if NET5_0

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static value128 FullMultiplication(ulong value1, ulong value2)
    {
      ulong hi = Math.BigMul(value1, value2, out ulong lo);
      return new value128(hi, lo);
    }

#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static value128 FullMultiplication(ulong value1, ulong value2)
    {
#if HAS_INTRINSICS
      if (System.Runtime.Intrinsics.X86.Bmi2.X64.IsSupported)
      {
        ulong lo;
        ulong hi = System.Runtime.Intrinsics.X86.Bmi2.X64.MultiplyNoFlags(value1, value2, &lo);
        return new value128(hi, lo);
      }
#endif
      return Emulate64x64to128(value1, value2);
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
      return (c > 32) ? false : Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(table), (nint)c);
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
    internal static bool strncasecmp(char* input1, string input2, int length)
    {
      fixed (char* p2 = input2)
      {
        return strncasecmp(input1, p2, length);
      }
    }

    internal static bool strncasecmp(char* input1, char* input2, int length)
    {
      int running_diff = 0;

      for (int i = 0; i < length; i++)
      {
        running_diff |= (input1[i] ^ input2[i]);
      }
      return (running_diff == 0) || (running_diff == 32);
    }
    internal static bool strncasecmp(byte* input1, ReadOnlySpan<byte> input2, int length)
    {
      int running_diff = 0;

      for (int i = 0; i < length; i++)
      {
        running_diff |= (input1[i] ^ input2[i]);
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
      {
        return 32 + Log2SoftwareFallback((uint)value);
      }
 
      return Log2SoftwareFallback(hi);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      static int Log2SoftwareFallback(uint value)
      {
        if (value == 0)
        {
          return 32;
        }
        
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
    public static float Int32BitsToSingle(int value)
#if HAS_BITOPERATIONS
      => BitConverter.Int32BitsToSingle(value);
#else
      => *((float*)&value);
#endif





#if HAS_INTRINSICS

    /// <summary>
    /// Detect eight consecutive digits and parse them a an unsigned int using SIMD instructions
    /// </summary>
    /// <param name="start">pointer to the sequence of char to evaluate</param>
    /// <param name="value">out : parsed value</param>
    /// <returns>bool : succes of operation : true meaning the sequence contains at least 8 consecutive digits</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryParseEightConsecutiveDigits_SIMD(char* start, out uint value)
    {
      value = 0;

      // escape if SIMD functions aren't available.
      if (!Sse41.IsSupported)
      {
        value = 0;
        return false;
      }




      value = 0;
      Vector128<short> raw = Sse41.LoadDquVector128((short*)start);
      int dsp = -1;

      var bytemask = Sse2.CompareEqual(raw.AsByte(), Vector128.Create((byte)'.')).AsByte();
      var mask = Sse41.MoveMask(bytemask);
      if (mask != 0)
      {
        if (!((mask & (mask - 1)) == 0))
        {
          return false;
        }
        var tzc = System.Numerics.BitOperations.TrailingZeroCount(mask);
        dsp = tzc / 2;
      }


      Vector128<short> ascii0 = Vector128.Create((short)(48 + short.MinValue));
      Vector128<short> after_ascii9 = Vector128.Create((short)(short.MinValue + 9));
      Vector128<short> a = Sse41.Subtract(raw, ascii0);
      Vector128<short> b = Sse41.CompareLessThan(after_ascii9, a);

      if (!Sse41.TestZ(b, b))
      {
        return false;
      }

      // @Credit  AQRIT
      // https://stackoverflow.com/questions/66371621/hardware-simd-parsing-in-c-sharp-performance-improvement/66430672
      Vector128<byte> mul1 = Vector128.Create(0x14C814C8, 0x010A0A64, 0, 0).AsByte();
      Vector128<short> mul2 = Vector128.Create(0x00FA61A8, 0x0001000A, 0, 0).AsInt16();

      //  extract the low bytes of each 16-bit word
      var vb = Sse41.Shuffle(a.AsByte(), Vector128.Create(shufflevectors_values[dsp+1]).AsByte());
      Vector128<int> v = Sse2.MultiplyAddAdjacent(Ssse3.MultiplyAddAdjacent(mul1, vb.AsSByte()), mul2);
      v = Sse2.Add(Sse2.Add(v, v), Sse2.Shuffle(v, 1));
      value = (uint)v.GetElement(0);

      return true;

    }

  

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T monVecteur<T>(T [] myArray,  int deplacement)
    {
      ref T tableRef =
         ref MemoryMarshal.GetArrayDataReference(myArray);
      return Unsafe.Add(ref tableRef, (IntPtr)(uint)deplacement);
    }


    internal unsafe struct SIMDParseResult
    {
      internal bool valid;
      internal ulong parsed_value;
      //internal int characters_consumed;
      internal bool decimal_separator_encountered;
      internal char* decimal_separator_position;
    //  internal bool exponent_encountered;
      internal int digit_count;
    }

    internal readonly static ulong[] shufflevectors_values =
        {
        1012195045828461056, // 0 no move
        1012195045828461311, // dsp =1 
        1012195045828460799, // dsp = 2... 
        1012195045828329727,
        1012195045794775295,
        1012195037204840703,
        1012192838181585151,
        1011629888228163839,
        867514700152307967
        };



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ParseStringWithSIMD_actual(char* start, char* end, out SIMDParseResult pns,   char? separator, bool allowExponent)
    {

      pns = new SIMDParseResult() { valid = false, parsed_value = 0,  decimal_separator_encountered = false };

      if (start == end || end-start < 8)
      {
        pns.valid = false;
        return false;
      }



        var dsp = -1;
        var raw = Sse41.LoadDquVector128((byte*)start);
        
        if (separator.HasValue)
        {
          var bytemask = Sse2.CompareEqual(raw, Vector128.Create((byte)separator)).AsByte();
          var  mask = Sse41.MoveMask(bytemask);
          if(!((mask & (mask - 1)) == 0))
          {
            pns.valid = false;
            return false;
          }
          var tzc = System.Numerics.BitOperations.TrailingZeroCount(mask);
          dsp = (mask == 0 ? -1 : tzc / 2);

      }

        int text_lenght = (Math.Min(8, (int)(end - start)));

        //pns.characters_consumed += text_lenght;
      

      Vector128<byte> ShuffleVector = Vector128.Create(shufflevectors_values[dsp+1]).AsByte();


      // test for e / E
      // if encountered, shrink parsed string and adjust cursor_position and consumed_characters
      //if (allowExponent)
      //{
      //  var bytemask = Sse2.CompareEqual(raw, Vector128.Create((byte)'e')).AsByte();
      //  var exp_mask = Sse41.MoveMask(bytemask);
      //  if (exp_mask == 0)
      //  {
      //    bytemask = Sse2.CompareEqual(raw, Vector128.Create((byte)'E')).AsByte();
      //    exp_mask = Sse41.MoveMask(bytemask);
      //  }
      //  if (exp_mask > 0)
      //  {
      //    //pns.exponent_encountered = true;
      //    var shrink_pos = exp_mask switch
      //    {
      //      (1) => 0,
      //      (4) => 1,
      //      (16) => 2,
      //      (64) => 3,
      //      (256) => 4,
      //      (1024) => 5,
      //      (4096) => 6,
      //      (16384) => 7,
      //      _ => -1
      //    };

      //    if (shrink_pos == -1)
      //    {
      //      pns.valid = false;
      //      return false;

      //    }

      //    //pns.characters_consumed -= text_lenght;
      //    //pns.characters_consumed += shrink_pos;

      //    text_lenght = shrink_pos;


      //  }
      //}


      if (dsp ==-2)
        {
          pns.valid = false;
          return false;
        }

      

        var a = Sse2.SubtractSaturate(raw, Vector128.Create((byte)'0'));
        Vector128<byte> after_ascii9 = Vector128.Create((byte)(byte.MinValue + 9));

        var vb = Sse41.Shuffle(a.AsByte(), ShuffleVector);
        //Vector128<long> shift_amount = Sse2.ConvertScalarToVector128Int32(8 - text_lenght << 3).AsInt64();
        //vb = Sse2.ShiftLeftLogical(vb.AsInt64(), shift_amount).AsByte();
        Vector128<sbyte> b = Sse41.CompareLessThan(after_ascii9.AsSByte(), vb.AsSByte());

        if (!Sse41.TestZ(b, b))
        {
          pns.valid = false;
          return false;
        }

        //// @Credit  AQRIT
        //// https://stackoverflow.com/questions/66371621/hardware-simd-parsing-in-c-sharp-performance-improvement/66430672
        Vector128<byte> mul1 = Vector128.Create(0x14C814C8, 0x010A0A64, 0, 0).AsByte();
        Vector128<short> mul2 = Vector128.Create(0x00FA61A8, 0x0001000A, 0, 0).AsInt16();

        ////  extract the low bytes of each 16-bit word
        Vector128<int> v = Sse2.MultiplyAddAdjacent(Ssse3.MultiplyAddAdjacent(mul1, vb.AsSByte()), mul2);
        v = Sse2.Add(Sse2.Add(v, v), Sse2.Shuffle(v, 1));

        if (dsp >= 0)
        {
          pns.decimal_separator_position = start+ dsp;
          pns.decimal_separator_encountered = true;
          text_lenght--;
        }

        pns.digit_count += text_lenght;

        

        pns.parsed_value = (uint)v.GetElement(0);
        pns.valid = true;

      return true;

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ParseStringWithSIMD(char* start, out SIMDParseResult pns, char? separator, bool allowExponent)
    {

      pns = new SIMDParseResult() { valid = false, parsed_value = 0,  decimal_separator_encountered = false };





      Vector128<short> raw = Sse41.LoadDquVector128((short*)start);
      int dsp = -1;


      var bytemask = Sse2.CompareEqual(raw.AsByte(), Vector128.Create((byte)'.')).AsByte();
      var mask = Sse41.MoveMask(bytemask);
      if (mask != 0)
      {
        if (!((mask & (mask - 1)) == 0))
        {
          return false;
        }
        var tzc = System.Numerics.BitOperations.TrailingZeroCount(mask);
        dsp = tzc / 2;
      }
      int text_lenght = 8;//(Math.Min(8, (int)(end - start)));

      Vector128<short> ascii0 = Vector128.Create((short)(48 + short.MinValue));
      Vector128<short> after_ascii9 = Vector128.Create((short)(short.MinValue + 9));
      Vector128<short> a = Sse41.Subtract(raw, ascii0);
      Vector128<short> b = Sse41.CompareLessThan(after_ascii9, a);

      var c=  Sse41.Shuffle(b.AsByte(), Vector128.Create(shufflevectors_values[dsp + 1]).AsByte());


      if (!Sse41.TestZ(c, c))
      {
        return false;
      }

      //// @Credit  AQRIT
      //// https://stackoverflow.com/questions/66371621/hardware-simd-parsing-in-c-sharp-performance-improvement/66430672
      Vector128<byte> mul1 = Vector128.Create(0x14C814C8, 0x010A0A64, 0, 0).AsByte();
      Vector128<short> mul2 = Vector128.Create(0x00FA61A8, 0x0001000A, 0, 0).AsInt16();

      ////  extract the low bytes of each 16-bit word
      var vb = Sse41.Shuffle(a.AsByte(), Vector128.Create(shufflevectors_values[dsp + 1]).AsByte());
      Vector128<int> v = Sse2.MultiplyAddAdjacent(Ssse3.MultiplyAddAdjacent(mul1, vb.AsSByte()), mul2);
      v = Sse2.Add(Sse2.Add(v, v), Sse2.Shuffle(v, 1));

      if (dsp >= 0)
      {
        pns.decimal_separator_position = start + dsp;
        pns.decimal_separator_encountered = true;
        text_lenght--;
      }

      pns.digit_count += text_lenght;



      pns.parsed_value = (uint)v.GetElement(0);
      pns.valid = true;

      return true;

    }



#endif

  }
}
