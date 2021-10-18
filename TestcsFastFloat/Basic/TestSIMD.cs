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


    [Fact]
    public void parse_eight_digits_simd_works_rnd()
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
            Assert.True(Utils.eval_parse_eight_digits_simd(pos, pos + sut.Length, out uint res));
            Assert.Equal(double.Parse(sut), res);
          }
        }

      }



    }


    [Fact]
    public void parse_eight_digits_simd_works()
    {

      Random RandNum = new Random();

      for (int i = 0; i <= 9; i++)
      {


        string sut = new string(i.ToString()[0], 8);
        unsafe
        {


          fixed (char* pos = sut)
          {
            Assert.True(Utils.eval_parse_eight_digits_simd(pos, pos + sut.Length, out uint res));

            Assert.Equal(double.Parse(sut), res);
          }


        }

      }



    }



    [Fact]
    public unsafe void eval_and_parse_eight_digits_simd_works()
    {


      for (int i = 1; i <= 9; i++)
      {

        string sut = new string(i.ToString()[0], 8);
        fixed (char* start = sut)
        {
          char* pos = start;
          Assert.True(Utils.eval_parse_eight_digits_simd(pos, pos + sut.Length, out uint res));
          Assert.Equal(double.Parse(sut), res);
        }
      }
    }

    [Fact]
    public unsafe void eval_and_parse_eight_digits_simd_works_rnd()
    {

      Random RandNum = new Random();

      for (int i = 0; i != 850000; i++)
      {

        int RandomNumber = RandNum.Next(10000000, 99999999);
        string sut = RandomNumber.ToString();

        fixed (char* start = sut)
        {
          char* pos = start;
          Assert.True(Utils.eval_parse_eight_digits_simd(pos, pos + sut.Length, out uint res));
          Assert.Equal(double.Parse(sut), res);
        }

      }
    }

    [Fact]
    public unsafe void eval_and_parse_eight_digits_simd2_works_rnd()
    {

      Random RandNum = new Random();

      for (int i = 0; i != 850000; i++)
      {

        int RandomNumber = RandNum.Next(10000000, 99999999);
        string sut = RandomNumber.ToString();

        fixed (char* start = sut)
        {
          char* pos = start;
          Assert.True(Utils.eval_parse_eight_digits_simd2(pos, pos + sut.Length, out uint res));
          Assert.Equal(double.Parse(sut), res);
        }

      }
    }

#endif
  }
}