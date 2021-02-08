using ApprovalUtilities.Persistence;
using csFastFloat;
using System;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace TestcsFastFloat.Tests.Exaustive
{
  public class FastParserTest
  {
    [Fact(Skip = "En attente d'un environ. de test")]
    unsafe private void All32BitsValues()
    {
      for (uint w = 0; w <= 0xFFFFFFFF; w++)
      {
        float f;
        uint word = (uint)(w);

        Buffer.MemoryCopy(&word, &f, sizeof(float), sizeof(float));

        string sut = f.ToString().Replace(",", ".");

        Assert.Equal(f, FastFloatParser.ParseFloat(sut));
      }
    }
  }
}