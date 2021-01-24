using csFastFloat;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using TestcsFastFloat.Tests.Basic;
using Xunit;

namespace TestcsFastFloat.Tests.Benchmarking
{
  public class TestCharge : BaseTestClass
  {
    // Bit mixer
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong rng(ulong h)
    {
      h ^= h >> 33;
      h *= (ulong)(0xff51afd7ed558ccd);
      h ^= h >> 33;
      h *= (ulong)(0xc4ceb9fe1a85ec53);
      h ^= h >> 33;
      return h;
    }

    private static void check(double d)
    {
      //std::string s(64, '\0');
      string s = new string('\0', 64);

      //auto written = std::snprintf(&s[0], s.size(), "%.*e", DBL_DIG + 1, d);
      //s.resize(written);
      s = d.ToString().Replace(",", ".");

      double? x = FastParser.ParseDouble(s);
      if (!x.HasValue)
      {
        throw new ParseException(s, "refused", 0, 0);
      }
      // if (isok != s.data() + s.size()) throw std::runtime_error("does not point at the end");
      if (d != x)
      {
        throw new ParseException(s, "disagree", x.Value, d);
      }
    }

    private static void check(float f)
    {
      //std::string s(64, '\0');
      string s = new string('\0', 64);

      //auto written = std::snprintf(&s[0], s.size(), "%.*e", DBL_DIG + 1, d);
      //s.resize(written);
      s = f.ToString().Replace(",", ".");

      float? x = FastParser.ParseFloat(s);
      if (!x.HasValue)
      {
        throw new ParseException(s, "refused", 0, 0);
      }
      // if (isok != s.data() + s.size()) throw std::runtime_error("does not point at the end");
      if (f != x)
      {
        throw new ParseException(s, "disagree", x.Value, f);
      }
    }

    [Fact]
    private unsafe void TestCheck_Random_Double()
    {
      var did = 0;
      var refused = 0;
      var disagree = 0;

      ulong offset = 1190;
      var howmany = 10000000;

      for (var i = 1; i <= howmany; i++)
      {
        // mix bits
        ulong x = rng((ulong)i + offset);
        double d;
        Buffer.MemoryCopy(&x, &d, sizeof(double), sizeof(double));
        // paranoid
        while ((!double.IsNormal(d)) || double.IsNaN(d) || double.IsInfinity(d))
        {
          offset++;
          x = rng((ulong)i + offset);
          Buffer.MemoryCopy(&x, &d, sizeof(double), sizeof(double));
        }
        try
        {
          did += 1;
          check(d);
        }
        catch (ParseException ex)
        {
          if (ex.Reason == "refused") refused += 1;
          if (ex.Reason == "disagree") disagree += 1;
        }
        catch
        {
          throw;
        }
      }
      Assert.Equal(0, refused);
      Assert.Equal(0, disagree);
    }

    [Fact]
    private unsafe void TestCheck_Random_Float()
    {
      var did = 0;
      var refused = 0;
      var disagree = 0;

      ulong offset = 1190;
      var howmany = 10000000;

      for (var i = 1; i <= howmany; i++)
      {
        // mix bits
        ulong x = rng((ulong)i + offset);
        float f;
        Buffer.MemoryCopy(&x, &f, sizeof(float), sizeof(float));
        // paranoid
        while ((!float.IsNormal(f)) || float.IsNaN(f) || float.IsInfinity(f))
        {
          offset++;
          x = rng((ulong)i + offset);
          Buffer.MemoryCopy(&x, &f, sizeof(float), sizeof(float));
        }
        try
        {
          did += 1;
          check(f);
        }
        catch (ParseException ex)
        {
          if (ex.Reason == "refused") refused += 1;
          if (ex.Reason == "disagree") disagree += 1;
        }
        catch
        {
          throw;
        }
      }
      Assert.Equal(0, refused);
      Assert.Equal(0, disagree);
    }

    [Fact]
    private unsafe void Bench1()
    {
      ulong offset = 1190;
      var howmany = 10000000;
      //   var start = DateTime.Now;

      for (var i = 1; i <= howmany; i++)
      {
        // mix bits
        ulong x = rng((ulong)i + offset);
        double d;
        Buffer.MemoryCopy(&x, &d, sizeof(double), sizeof(double));
        // paranoid
        while ((!double.IsNormal(d)) || double.IsNaN(d) || double.IsInfinity(d))
        {
          offset++;
          x = rng((ulong)i + offset);
          Buffer.MemoryCopy(&x, &d, sizeof(double), sizeof(double));
        }

        double var = FastParser.ParseDouble(d.ToString(CultureInfo.CurrentCulture));
      }

      //  ApprovalTests.Approvals.Verify(DateTime.Now - start);
    }

    [Fact]
    private unsafe void Bench2()
    {
      ulong offset = 1190;
      var howmany = 10000000;

      for (var i = 1; i <= howmany; i++)
      {
        // mix bits
        ulong x = rng((ulong)i + offset);
        double d;
        Buffer.MemoryCopy(&x, &d, sizeof(double), sizeof(double));
        // paranoid
        while ((!double.IsNormal(d)) || double.IsNaN(d) || double.IsInfinity(d))
        {
          offset++;
          x = rng((ulong)i + offset);
          Buffer.MemoryCopy(&x, &d, sizeof(double), sizeof(double));
        }
        try
        {
          double var = double.Parse(d.ToString(CultureInfo.CurrentCulture));
        }
        catch
        {
          throw;
        }
      }
    }
  }
}