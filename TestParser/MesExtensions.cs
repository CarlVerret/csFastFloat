using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;

namespace cs_FastFloat.Tests
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