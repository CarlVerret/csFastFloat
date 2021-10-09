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


#if NET5_0_OR_GREATER

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

            Assert.True(Utils.eval_parse_eight_digits_simd(&pos , pos+ sut.Length, out uint res));
            Assert.Equal(8, pos- start);
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

            Assert.True(Utils.eval_parse_eight_digits_simd(&pos, pos + sut.Length, out uint res));
            Assert.Equal(8, pos - start);
            Assert.Equal(double.Parse(sut), res);
          }




        }

      }



    }


    [SkippableFact]
    unsafe public void ParseNumberString_Works_Scnenarios()
    {
      Skip.If(base.NoDiffToolDetected(), "No diff tool detected");

      Dictionary<string, string> sut = new Dictionary<string, string>();

      sut.Add("leading zeros", "001");
      sut.Add("leading zeros neg", "-001");

      sut.Add("zero", "0");
      sut.Add("double", "0.00000000000000212312312");
      sut.Add("double neg", "-0.00000000000000212312312");
      sut.Add("int", "1");
      sut.Add("int neg", "-1");

      sut.Add("autreint ", "123124");
      sut.Add("autreint neg", "-123124");

      sut.Add("notation scientifique", "4.56E+2");
      sut.Add("notation scientifique neg", "-4.56E-2");

      sut.Add("notation scientifique 2", "4.5644E+2");
      sut.Add("notation scientifique 2 neg", "-4.5644E-2");

      sut.Add("notation scientifique 3", "4424.5644E+22");
      sut.Add("notation scientifique 3 neg", "-4424.5644E-22");

      sut.Add("notation scientifique 4", "4424.5644E+223");
      sut.Add("notation scientifique 4 neg", "-4424.5644E-223");
      StringBuilder sb = new StringBuilder();

      foreach (KeyValuePair<string, string> kv in sut)
      {
        sb.AppendLine($"Scenario : {kv.Key} ");
        sb.AppendLine($"Valeur   : {kv.Value} ");

        fixed (char* p = kv.Value)
        {
          char* pend = p + kv.Value.Length;
          var res = ParsedNumberString.ParseNumberStringSIMD2(p, pend);

          sb.AppendLine($"Resultat : {res.exponent} {res.mantissa} {res.negative} {res.valid}");
          sb.AppendLine();
        }
      }

      // We do not want to fail the tests when the user has not
      // configured a diff tool.
      try
      {
        VerifyData(sb.ToString());
      }
      catch (System.Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }





#endif

  }
}
