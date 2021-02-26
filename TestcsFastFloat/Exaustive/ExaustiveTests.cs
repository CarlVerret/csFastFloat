using ApprovalUtilities.Persistence;
using csFastFloat;
using System;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace TestcsFastFloat.Tests.Exaustive
{
  public class FastParserTest
  {
    [Fact(Skip = "Waiting for test environ.")]
    unsafe private void All32BitsValues()
    {
      for (uint w = 0; w <= 0xFFFFFFFF; w++)
      {
        uint word = (uint)(w);
        float f = BitConverter.Int32BitsToSingle((int)word);

        string sut = f.ToString().Replace(",", ".");

        Assert.Equal(f, FastFloatParser.ParseFloat(sut));
      }
    }
  }
}