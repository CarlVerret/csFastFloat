using csFastFloat;
using System;
using TestcsFastFloat.Tests;
using Xunit;

#if NET5_0_OR_GREATER
using System.Runtime.Intrinsics.X86;
#endif



namespace TestcsFastFloat.Basic.SIMD
{
  public class TestsSIMD : BaseTestClass
  {


#if NET5_0_OR_GREATER



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

    [SkippableFact]
    public unsafe void EvalAndParseEightDigits_SIMD_ReportsConsumedCount()
    {
      Skip.If(!Sse41.IsSupported, "No SIMD support");

      fixed (char* pos = "12345678")
      {
        Assert.True(Utils.TryParseEightConsecutiveDigits_SIMD(pos, out uint value, out int consumed));
        Assert.Equal(12345678u, value);
        Assert.Equal(8, consumed);
      }

      fixed (char* pos = "1234567.")
      {
        Assert.False(Utils.TryParseEightConsecutiveDigits_SIMD(pos, out uint _, out int consumed));
        Assert.Equal(0, consumed);
      }
    }

    [SkippableFact]
    public void ParseDouble_Through_SIMD_falls_back_on_decimal_point()
    {
      Skip.If(!Sse41.IsSupported, "No SIMD support");

      string sut = "12345678.12345678";
      double expected = 12345678.12345678d;
      double actual = FastDoubleParser.ParseDouble(sut);
      Assert.Equal(expected, actual, 10);
    }



    [SkippableFact]
    public void EvalAndParseEightDigits_SIMD_works_RandomInput()
    {

      Skip.If(!Sse41.IsSupported, "No SIMD support");



      Random RandNum = new Random();

      for (int i = 0; i != 850000; i++)
      {

        int RandomNumber = RandNum.Next(10000000, 99999999);

        string sut = RandomNumber.ToString();
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








#endif
  }
}
