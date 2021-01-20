using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Newtonsoft.Json;
using System.Text;

namespace cs_fast_double_parser.Tests
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
  }
}