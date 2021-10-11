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


    [InlineData( "00000000", true)]
    [InlineData("99999999", true)]
    [InlineData("999999999", true)]
    [InlineData("1", false)]
    [InlineData("a", false)]
    [InlineData(":00000000", false)]
    [InlineData("/00000000", false)]
    [Theory]
    public void eval_scenarios(string sut, bool res)
    {


      unsafe
      {
        fixed (char* start = sut)
        {
          char* pos = start;

          Assert.Equal(res, Utils.eval_parse_eight_digits_simd(start, start + sut.Length, out uint d));

          //Assert.Equal(8, pos- start);
          //Assert.Equal(double.Parse(sut), res);
        }




      }
    }

    [Fact]
    public void eval_and_parse_eight_digits_simd_works()
    {

   
      for (int i = 1; i <= 9; i++)
      {


        string sut =  new string(i.ToString()[0], 8);
        unsafe
        {
          fixed (char* start = sut)
          {
            char* pos = start;

            Assert.True(Utils.eval_parse_eight_digits_simd(start, start + sut.Length, out uint res));

            //Assert.Equal(8, pos- start);
            Assert.Equal(double.Parse(sut), res);
          }

     


        }

      }



    }

    [Fact]
    public void eval_and_parse_eight_digits_simd_works_rnd()
    {

      Random RandNum = new Random();

      for (int i = 0; i != 850000; i++)
      {

        int RandomNumber = RandNum.Next(10000000, 99999999);

        string sut = RandomNumber.ToString();
        unsafe
        {
          fixed (char* start = sut)
          {
            char* pos = start;

            Assert.True(Utils.eval_parse_eight_digits_simd(pos, pos + sut.Length,  out uint res));
          //  Assert.Equal(8, pos - start);
            Assert.Equal(double.Parse(sut), res);
          }




        }

      }



    }
        
#endif
  }


}