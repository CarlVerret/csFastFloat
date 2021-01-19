using ApprovalTests.Reporters;
using MonFloatParser;
using System;
using Xunit;

namespace TestParser
{
  public class TestUtils : BaseTestClass
  {
    [Fact]
    public void as_char_EOF()
    {
      Assert.Equal('\0', Utils.as_char("", 1));
      Assert.Equal('\0', Utils.as_char("a", 2));
    }

    [Fact]
    public void as_char_fonctionne()
    {
      string sut = "12345";
      for (int i = 0; i != sut.Length; i++)
      {
        Assert.Equal(sut[i], Utils.as_char(sut, i));
      }
    }

    [Fact]
    public void as_digit_fonctionne()
    {
      string sut = "0123456789";
      for (int i = 0; i != sut.Length; i++)
        Assert.Equal(i, Utils.as_digit(sut, i));
    }

    [Fact]
    public void is_integer_fonctionne_int()
    {
      string sut = "0123456789";
      for (int i = 0; i != sut.Length; i++)
        Assert.True(Utils.is_integer(sut, i));
    }

    [Fact]
    public void is_integer_fonctionne_else()
    {
      string sut = " .Eeaabbcc";
      for (int i = 0; i != sut.Length; i++)
        Assert.False(Utils.is_integer(sut, i));
    }
  }
}