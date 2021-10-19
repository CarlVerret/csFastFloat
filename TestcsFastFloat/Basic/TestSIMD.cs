using csFastFloat;
using csFastFloat.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestcsFastFloat.Tests;
using Xunit;

namespace TestcsFastFloat.Basic.SIMD
{
  public class TestsSIMD : BaseTestClass
  {


#if NET5_0



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
    public unsafe void EvalAndParseEightDigits_SIMD_works_Scenarios(bool shouldPass, string sut )
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
    {
          fixed (char* pos = sut)
          {
            Assert.Equal(shouldPass, Utils.TryParseEightConsecutiveDigits_SIMD(pos, out uint res));
          }

    }




    [Fact]
    public void EvalAndParseEightDigits_SIMD_works_RandomInput()
    {

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


    [Fact]
    public void parse_eight_digits_simd_works()
    {

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



   

    [Fact]
    public unsafe void EvalAndParseEightDigits_SIMD2()
    {

      Random RandNum = new Random();

      for (int i = 0; i != 850000; i++)
      {

        int RandomNumber = RandNum.Next(10000000, 99999999);
        string sut = RandomNumber.ToString();

        fixed (char* start = sut)
        {
          char* pos = start;
          Assert.True(Utils.TryParseEightConsecutiveDigits_SIMD(pos, out uint res));
          Assert.Equal(double.Parse(sut), res);
        }

      }
    }

#endif
  }
}