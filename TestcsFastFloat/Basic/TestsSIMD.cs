﻿using csFastFloat;
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


#if HAS_INTRINSICS

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
          fixed (char* start = sut)
          {
            Assert.True(Utils.is_made_of_eight_digits_fast_simd(start));
            uint res = Utils.parse_eight_digits_simd(start);
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
          fixed (char* start = sut)
          {
            Assert.True(Utils.is_made_of_eight_digits_fast_simd(start));
            uint res = Utils.parse_eight_digits_simd(start);
            Assert.Equal(double.Parse(sut), res);
          }

          fixed (char* start = sut)
          {
            Assert.True(Utils.is_made_of_eight_digits_fast_simd(start));
            uint res = Utils.parse_eight_digits_simd(start);
            Assert.Equal(double.Parse(sut), res);
          }


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

            Assert.True(Utils.is_made_of_eight_digits_fast_simd(start));

            Assert.True(Utils.eval_parse_eight_digits_simd(pos , pos+ sut.Length, out uint res));
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