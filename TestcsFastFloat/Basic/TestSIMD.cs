using csFastFloat;
using csFastFloat.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using TestcsFastFloat.Tests;
using Xunit;

using System.Runtime.Intrinsics.X86;
using static csFastFloat.Utils;
using System.Runtime.Intrinsics;

namespace TestcsFastFloat.Basic.SIMD
{
  public class TestsSIMD : BaseTestClass
  {
    internal static Vector128<byte>[] shufflevectors = new Vector128<byte>[]
    {
               Vector128.Create( 0, 2, 4, 6, 8, 10, 12, 14, 0, 2, 4, 6, 8, 10, 12, 14 ).AsByte(),
               Vector128.Create( -1, 2, 4, 6, 8, 10, 12, 14, -1, 2, 4, 6, 8, 10, 12, 14).AsByte(),
               Vector128.Create(-1, 0, 4, 6, 8, 10, 12, 14, -1, 0, 4, 6, 8, 10, 12, 14).AsByte(),
               Vector128.Create(-1, 0, 2, 6, 8, 10, 12, 14, -1, 0, 2, 6, 8, 10, 12, 14).AsByte(),
               Vector128.Create(-1, 0, 2, 4, 8, 10, 12, 14, -1, 0, 2, 4, 8, 10, 12, 14).AsByte(),
               Vector128.Create(-1, 0, 2, 4, 6, 10, 12, 14, -1, 0, 2, 4, 6, 10, 12, 14).AsByte(),
               Vector128.Create(-1, 0, 2, 4, 6, 8, 12, 14, -1, 0, 2, 4, 6, 8, 12, 14).AsByte(),
               Vector128.Create(- 1, 0, 2, 4, 6, 8, 10, 14, -1, 0, 2, 4, 6, 8, 10, 14).AsByte(),
               Vector128.Create(- 1, 0, 2, 4, 6, 8, 10, 12, -1, 0, 2, 4, 6, 8, 10, 12).AsByte()
    };


    internal static ulong[] shufflevectors_values = new ulong[]
      {
        1012195045828461056,
        1012195045828461311,
        1012195045828460799,
        1012195045828329727,
        1012195045794775295,
        1012195037204840703,
        1012192838181585151,
        1011629888228163839,
        867514700152307967
      };



    //[Fact]
    //unsafe public void test()
    //{
    //  StringBuilder sb = new StringBuilder();

    //  string[] liste = new[]
    //  {
    //    "12345678",
    //    ".12345678",
    //    "1.2345678",
    //    "12.345678",
    //    "123.45678",
    //    "1234.5678",
    //    "12345.678",
    //    "123456.78",
    //    "1234567.8",
    //    "1.234.5678."
    //  };

    //  foreach (string item in liste)
    //  {
    //    fixed (char* start = item)
    //    {
    //      var raw = Sse41.LoadDquVector128((byte*)start);

    //      var bytemask = Sse2.CompareEqual(raw, Vector128.Create((byte)'.')).AsByte();
    //      var mask = Sse41.MoveMask(bytemask);

    //      Vector128<byte>? ShuffleVector= default(Vector128<byte>);
    //      Vector128<byte>? ShuffleVector2= default(Vector128<byte>);


    //      var tzc = System.Numerics.BitOperations.TrailingZeroCount(mask);
    //      var dsp = ( mask ==0 ? -1 : tzc / 2);
         
    //      if((mask & (mask - 1)) == 0)
    //      {
    //        ShuffleVector = shufflevectors[dsp+1];
    //        ShuffleVector2 = Vector128.Create(shufflevectors_values[dsp + 1]).AsByte();

    //      }
    //      sb.AppendLine($"{item} {mask} {tzc} {dsp}");
    //      sb.AppendLine($"1- {ShuffleVector}");
    //      sb.AppendLine($"2- {ShuffleVector2}");


    //    }

    //  }

    //  VerifyData(sb.ToString());

    //}


    [InlineData(true, "12345678")]
    [InlineData(true, "123456789")]
    [InlineData(false, "1")]
    [InlineData(false, "12")]
    [InlineData(false, "123")]
    [InlineData(false, "1234")]
    [InlineData(false, "12345")]
    [InlineData(false, "123456")]
    [InlineData(false, "1234567")]
    [InlineData(false, "/1234567")]
    [InlineData(false, ":1234567")]
    [InlineData(false, "1:234567")]
    [InlineData(false, "1234567:")]
    [InlineData(false, "1234567 ")]
    [InlineData(false, "1234 567")]
    [Theory]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
    public unsafe void EvalAndParseEightDigits_SIMD_works_Scenarios(bool shouldPass, string sut)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
    {
      Skip.If(!Sse41.IsSupported, "No SIMD support");
      fixed (char* pos = sut)
      {
        Assert.Equal(shouldPass, Utils.TryParseEightConsecutiveDigits_SIMD(pos, out uint res));
      }

    }
       
    //[InlineData("1", -1, "1")]
    //[InlineData("11", -1, "11")]
    //[InlineData("111", -1, "111")]
    //[InlineData("1111", -1, "1111")]
    //[InlineData("11111", -1, "11111")]
    //[InlineData("111111", -1, "111111")]
    //[InlineData("1111111", -1, "1111111")]
    //[InlineData("11111111", -1, "11111111")]
    //[InlineData("12345678",-1 ,"12345678")]
    [InlineData(".12345678", 0,"1234567")]
		[InlineData("1.2345678", 1, "1234567")]
		[InlineData("12.345678", 2, "1234567")]
		[InlineData("123.45678", 3, "1234567")]
		[InlineData("1234.5678", 4, "1234567")]
		[InlineData("12345.678", 5, "1234567")]
		[InlineData("123456.78", 6, "1234567")]
		[InlineData("1234567.8", 7, "1234567")]
		[InlineData("12345678.", -1, "12345678")]
		[Theory]
    public void EvalAndParseEightDigits_SIMD_WithSeparator_Works(string sut, int decimal_sep_position, string expected)
    {
           
        unsafe
        {
          fixed (char* pos = sut)
          {

          Utils.ParseStringWithSIMD(pos, out SIMDParseResult res,  '.', true);

          Assert.True(res.valid);
          Assert.Equal(double.Parse(expected), res.parsed_value);
          if (decimal_sep_position >= 0)
          { 
            Assert.True(pos + decimal_sep_position == res.decimal_separator_position);
          }

          }
        }
      
    }






    [Fact]
    public void tt()
    {

      for (int i = 1; i < 15; i++)
      {
        var sut = "-65.613616999999977";

        unsafe
        {
          fixed (char* pos = sut)
          {


            var res = ParsedNumberString.ParseNumberString(pos, pos + sut.Length, decimal_separator: '.');
            var res2 = ParsedNumberString.ParseNumberString2(pos, pos + sut.Length, decimal_separator: '.');
           
            Assert.Equal(res,res2);
          }
        }
      }

    }







    [InlineData("")]
    [InlineData("a")]
    [InlineData("1a")]
    [InlineData("a1")]
    [InlineData("a 1")]
    [InlineData("1 1")]
    [InlineData("1/")]
    [InlineData("1:")]
    [InlineData("a2345678")]
    [InlineData("a.2345678")]
    [InlineData("a2.345678")]
    [InlineData("a23.45678")]
    [InlineData("a234.5678")]
    [InlineData("a2345.678")]
    [InlineData("a23456.78")]
    [InlineData("a234567.8")]
    [InlineData("a2345678.")]
    [InlineData(".a2345678")]
    [InlineData("123☺5678")]
    [InlineData("123..5678")]
    [InlineData("123:5678")]
    [SkippableTheory]
    public void EvalAndParseEightDigits_SIMD_WithSeparator_fails(string sut)
    {

      Skip.If(!Sse41.IsSupported, "No SIMD support");

      unsafe
      {
        fixed (char* pos = sut)
        {

          Utils.ParseStringWithSIMD(pos, out SIMDParseResult res, '.', true);
          Assert.False(res.valid);
        }
      }

    }




    [SkippableFact]
    public void parse_eight_digits_simd_works()
    {
      Skip.If(!Sse41.IsSupported, "No SIMD support");

      for (int i = 0; i <= 9; i++)
      {


        string sut = new string(i.ToString()[0], 8);
        unsafe
        {


          fixed (char* pos = sut)
          {
            Assert.True(Utils.TryParseEightConsecutiveDigits_SIMD(pos, out uint res));

            Assert.Equal(double.Parse(sut), res);
          }


        }

      }



    }







 

  }
}