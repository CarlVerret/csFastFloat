using csFastFloat;
using csFastFloat.Enums;
using csFastFloat.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestcsFastFloat.Tests.Basic
{
  public class TestFloatParser : BaseTestClass
  {
    [Trait("Category", "Smoke Test")]
    [Theory]
    [InlineData("nan", double.NaN)]
    [InlineData("inf", double.PositiveInfinity)]
    [InlineData("+nan", double.NaN)]
    [InlineData("-nan", double.NaN)]
    [InlineData("+inf", double.PositiveInfinity)]
    [InlineData("-inf", double.NegativeInfinity)]
    [InlineData("infinity", double.PositiveInfinity)]
    [InlineData("+infinity", double.PositiveInfinity)]
    [InlineData("-infinity", double.NegativeInfinity)]
    unsafe public void DoubleParser_HandleInvalidInput_works(string input, double res)
    {
      fixed (char* p = input)
      {
        Assert.Equal(res, FastDoubleParser.HandleInvalidInput(p, p + input.Length, out int _));
      }
    }

    [Trait("Category", "Smoke Test")]
    [Theory]
    [InlineData("nan", float.NaN)]
    [InlineData("inf", float.PositiveInfinity)]
    [InlineData("+nan", float.NaN)]
    [InlineData("-nan", float.NaN)]
    [InlineData("+inf", float.PositiveInfinity)]
    [InlineData("-inf", float.NegativeInfinity)]
    [InlineData("infinity", float.PositiveInfinity)]
    [InlineData("+infinity", float.PositiveInfinity)]
    [InlineData("-infinity", float.NegativeInfinity)]
    unsafe public void FloatParser_HandleInvalidInput_works(string input, float res)
    {
      fixed (char* p = input)
      {
        Assert.Equal(res, (float)FastDoubleParser.HandleInvalidInput(p, p + input.Length, out int _)); ;
      }
    }

    [Trait("Category", "Smoke Test")]
    [Fact]
    public void cas_compute_float_32_1()
    {
      for (int p = -65; p <= 38; p++)
      {
        if (p == 23)
          p++;

        var am = FastFloatParser.ComputeFloat(q: p, w: 1);

        float? f = FastFloatParser.ToFloat(false, am);

        if (!f.HasValue)
          throw new ApplicationException($"Can't parse p=> {p}");

        if (f != testing_power_of_ten_float[p + 65])
          throw new ApplicationException($"bad parsing p=> {p}");
      }
    }

    [Trait("Category", "Smoke Test")]
    [Fact]
    unsafe public void cas_compute_float_32_2()
    {
      for (int p = -1000; p <= 38; p++)
      {
        string sut = $"1e{p}";
        fixed (char* pstart = sut)
        {
          float? f = FastFloatParser.ParseFloat(pstart, pstart + sut.Length, chars_format.is_general);

          if (!f.HasValue)
          {
            throw new ApplicationException($"Can't parse p=> 1e{p}");
          }

          float expected = ((p >= -65) ? testing_power_of_ten_float[p + 65] : (float)Math.Pow(10, p));
          Assert.Equal(expected, f.Value);
        }
      }
    }

    [Fact]
    unsafe public void ParseNumberString_Works_Scnenarios()
    {
      Dictionary<string, string> sut = new Dictionary<string, string>();

      sut.Add("leading zeros", "001");
      sut.Add("leading zeros neg", "-001");

      sut.Add("zero", "0");
      sut.Add("zero neg", "-0");

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
          var res = ParsedNumberString.ParseNumberString(p, pend);

          sb.AppendLine($"Resultat : {res.exponent} {res.mantissa} {res.negative} {res.valid}");
          sb.AppendLine();
        }
      }

      VerifyData(sb.ToString());
    }

    [Fact]
    unsafe public void HandleNullValue() => Assert.Throws<System.ArgumentNullException>(() => Double.Parse(null));

    [Fact]
    unsafe public void HandleEmptyString() => Assert.Throws<System.FormatException>(() => Double.Parse(string.Empty));

    [Fact]
    unsafe public void ParseNumber_Works_Scenarios()
    {
      Dictionary<string, string> sut = new Dictionary<string, string>();

      sut.Add("leading spaces", "  1");
      sut.Add("leading spaces neg", "  -1");

      sut.Add("leading zeros", "001");
      sut.Add("leading zeros neg", "-001");

      sut.Add("zero", "0");
      sut.Add("zero neg", "-0");

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
          var res = FastDoubleParser.ParseNumber(p, pend, out int _);

          sb.AppendLine($"Resultat : {res}");
          sb.AppendLine();
        }
      }

      VerifyData(sb.ToString());
    }

    private static float[] testing_power_of_ten_float =  {
    1e-65F,
    1e-64F,  1e-63F,  1e-62F,  1e-61F,  1e-60F,  1e-59F,  1e-58F,  1e-57F,  1e-56F,
    1e-55F,  1e-54F,  1e-53F,  1e-52F,  1e-51F,  1e-50F,  1e-49F,  1e-48F,  1e-47F,
    1e-46F,  1e-45F,  1e-44F,  1e-43F,  1e-42F,  1e-41F,  1e-40F,  1e-39F,  1e-38F,
    1e-37F,  1e-36F,  1e-35F,  1e-34F,  1e-33F,  1e-32F,  1e-31F,  1e-30F,  1e-29F,
    1e-28F,  1e-27F,  1e-26F,  1e-25F,  1e-24F,  1e-23F,  1e-22F,  1e-21F,  1e-20F,
    1e-19F,  1e-18F,  1e-17F,  1e-16F,  1e-15F,  1e-14F,  1e-13F,  1e-12F,  1e-11F,
    1e-10F,  1e-9F,   1e-8F,   1e-7F,   1e-6F,   1e-5F,   1e-4F,   1e-3F,   1e-2F,
    1e-1F,   1e0F,    1e1F,    1e2F,    1e3F,    1e4F,    1e5F,    1e6F,    1e7F,
    1e8F,    1e9F,    1e10F,   1e11F,   1e12F,   1e13F,   1e14F,   1e15F,   1e16F,
    1e17F,   1e18F,   1e19F,   1e20F,   1e21F,   1e22F,   1e23F,   1e24F,   1e25F,
    1e26F,   1e27F,   1e28F,   1e29F,   1e30F,   1e31F,   1e32F,   1e33F,   1e34F,
    1e35F,   1e36F,   1e37F,   1e38F};
  }
}