using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Newtonsoft.Json;
using System;
using System.Text;

namespace TestcsFastFloat.Tests
{
  [UseReporter(typeof(DiffReporter))]
  public class BaseTestClass
  {
    public static void VerifyData<T>(T data, string nomScenario = "")
    {
      if (!string.IsNullOrEmpty(nomScenario))
      {
        using (ApprovalResults.ForScenario(nomScenario))
        {
          Approvals.Verify(data);
        }
      }
      else
      {
        Approvals.Verify(data);
      }
    }

    /// @Credit Dmitry Bychenko
    /// https://stackoverflow.com/questions/35449339/c-sharp-converting-from-float-to-hexstring-via-ieee-754-and-back-to-float/35450540#35450540
    internal static short ShortFromHexString(string s)
    {
      var i = Convert.ToInt16(s, 16);
      var bytes = BitConverter.GetBytes(i);
      return BitConverter.ToInt16(bytes, 0);
    }

    internal static float FloatFromHexString(string s)
    {
      var i = Convert.ToInt32(s, 16);
      var bytes = BitConverter.GetBytes(i);
      return BitConverter.ToSingle(bytes, 0);
    }

    internal static double DoubleFromHexString(string s)
    {
      var i = Convert.ToInt64(s, 16);
      var bytes = BitConverter.GetBytes(i);
      return BitConverter.ToDouble(bytes, 0);
    }
  }
}