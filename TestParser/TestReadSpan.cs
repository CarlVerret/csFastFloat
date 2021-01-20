using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace cs_fast_double_parser.Tests
{
  public class TestReadSpan : BaseTestClass
  {
    [InlineData("1a")]
    [InlineData("-1a")]
    [InlineData("1.1a")]
    [InlineData("0.000a1")]
    [InlineData("-0.000a1")]
    [InlineData("0a")]
    [InlineData("-0a")]
    [InlineData("0.")]
    [InlineData("-0.")]
    [InlineData("1.a")]
    [InlineData("-1.a")]
    [InlineData("4.56E+")]
    [InlineData("-4.56E+")]
    [InlineData("4.56E")]
    [InlineData("-4.56E")]
    [InlineData("4.56E*")]
    [InlineData("-4.56E*")]
    [InlineData("4.56E+a")]
    [InlineData("-4.56E+a")]
    [InlineData("a")]
    [InlineData("-a")]
    [Theory]
    public void parse_throws_NaN(string sut)
                       => Assert.Throws<System.FormatException>(() => DoubleParser.try_read_span(sut));

    [InlineData("1a")]
    [InlineData("-1a")]
    [InlineData("1.1a")]
    [InlineData("0.000a1")]
    [InlineData("-0.000a1")]
    [InlineData("0a")]
    [InlineData("-0a")]
    [InlineData("0.")]
    [InlineData("-0.")]
    [InlineData("1.a")]
    [InlineData("-1.a")]
    [InlineData("4.56E+")]
    [InlineData("-4.56E+")]
    [InlineData("4.56E")]
    [InlineData("-4.56E")]
    [InlineData("4.56E*")]
    [InlineData("-4.56E*")]
    [InlineData("4.56E+a")]
    [InlineData("-4.56E+a")]
    [InlineData("a")]
    [InlineData("-a")]
    [Theory]
    public void acutal_throws_NaN(string sut)
                       => Assert.Throws<System.FormatException>(() => Double.Parse(sut));

    [InlineData("10000000000000000000000000000000000000000000e+308")]
    [InlineData("3.1415926535897932384626433832795028841971693993751")]
    [InlineData("1.1e-327")]
    [InlineData("1.1e310")]
    [Theory]
    public void parse_throws_ParseRefused(string sut)
                        => Assert.Throws<DoubleParser.ParsingRefusedException>(() => DoubleParser.try_read_span(sut));

    [Fact]
    public void read_span_works_when_number()
    {
      Dictionary<string, string> sut = new Dictionary<string, string>();

      // TODO:
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

      sut.Add("overflow", "7.2057594037927933e+16"); // pas ce qu'on recoit en c++

      StringBuilder sb = new StringBuilder();

      foreach (KeyValuePair<string, string> kv in sut)
      {
        sb.AppendLine($"Scenario : {kv.Key} ");
        sb.AppendLine($"Valeur   : {kv.Value} ");
        sb.AppendLine($"Resultat : {DoubleParser.parse_number2(kv.Value)}");
      }

      VerifyData(sb.ToString());
    }
  }
}