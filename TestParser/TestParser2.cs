using ApprovalTests.Reporters;
using MonFloatParser;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace TestParser
{
  [UseReporter(typeof(DiffReporter))]
  public class ParseException : Exception
  {
    public string Value;
    public string Reason;
    private double _x;
    private double _d;

    public ParseException(string v, string reason, double x, double d)
    {
      Value = v;
      Reason = reason;
      _x = x;
      _d = d;
    }
  }

  public class TestParser2 : BaseTestClass
  {
    //// ulp distance
    //// Marc B. Reynolds, 2016-2019
    //// Public Domain under http://unlicense.org, see link for details.
    //// adapted by D. Lemire
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private unsafe ulong f64_ulp_dist(double a, double b)
    //{
    //  ulong ua;
    //  ulong ub;

    //  Buffer.MemoryCopy(&a, &ua, sizeof(double), sizeof(double));
    //  Buffer.MemoryCopy(&b, &ub, sizeof(double), sizeof(double));

    //  if ((ulong)(ub ^ ua) >= 0)
    //    return (ulong)(ua - ub) >= 0 ? (ua - ub) : (ub - ua);
    //  return ua + ub + 0x80000000;
    //}

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

      double? x = MonFloatParser.DoubleParser.parse_number(s);
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

    private void check_string(string s)
    {
      double? x = MonFloatParser.DoubleParser.parse_number(s);
      double? d = Double.Parse(s, CultureInfo.InvariantCulture);

      if (!x.HasValue)
      {
        throw new ParseException(s, "refused", 0, 0);
      }
      // if (isok != s.data() + s.size()) throw std::runtime_error("does not point at the end");
      if (d != x)
      {
        throw new ParseException(s, "disagree", x.Value, d.Value);
      }
    }

    [Fact]
    private unsafe void Test_Charge()
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
      ApprovalTests.Approvals.Verify($"Did :{did} Refused: {refused} Disagree{disagree}");
    }

    [Fact]
    private unsafe void Benchmark_1()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Start");

      ulong offset = 1190;
      var howmany = 10000000;
      var did = 0;
      var refused = 0;
      var disagree = 0;
      for (var j = 1; j <= 20; j++)
      {
        var start = DateTime.Now;
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
            // DoubleParser.parse_number(d.ToString().Replace(",", "."));
            Double.Parse(d.ToString());
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

        var end = DateTime.Now;

        sb.AppendLine($"Did :{did} Refused: {refused} Disagree{disagree} time {end - start}");
      }
      ApprovalTests.Approvals.Verify(sb.ToString());
    }

    [Fact(Skip = "on demand")]
    private unsafe void Benchmark_2()
    {
      var did = 0;
      var refused = 0;
      var disagree = 0;
      var start = DateTime.Now;

      ulong offset = 1190;
      var howmany = 50000000;

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
          double.Parse(d.ToString());
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

      var end = DateTime.Now;

      ApprovalTests.Approvals.Verify($"Did :{did} Refused: {refused} Disagree{disagree} time {end - start}");
    }
  }
}