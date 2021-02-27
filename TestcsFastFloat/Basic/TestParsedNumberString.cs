using csFastFloat.Structures;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestcsFastFloat.Tests.Basic
{
#pragma warning disable xUnit1026



  public class TestParsedNumberString : BaseTestClass
  {
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


  }
}