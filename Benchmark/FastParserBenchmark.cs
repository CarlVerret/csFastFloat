using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

//using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using csFastFloat.Structures;
using System;
using System.Globalization;



using BenchmarkDotNet.Reports;
using System.Collections.Generic;

namespace csFastFloat.Benchmark
{

  //[MemoryDiagnoser]
  [SimpleJob(RuntimeMoniker.NetCoreApp50)]
[Config(typeof(Config))]
public class FFBencmark
{
  private string[] _lines;
  private byte[][] _linesUtf8;

  private class Config : ManualConfig
  {
    public Config()
    {
      AddColumn(StatisticColumn.Min);
      AddColumn(new MFloatPerSecColumn());
      AddColumn(new VolumePerSecColumn());
    }
  }

  [Benchmark(Description = "Utf8Parser")]
  public double Utf8Parser()
  {
    double max = double.MinValue;

    foreach (byte[] l in _linesUtf8)
    {
      if (!System.Buffers.Text.Utf8Parser.TryParse(l, out double d, out int consumed) || consumed != l.Length)
        throw new InvalidOperationException();

      max = d > max ? d : max;
    }
    return max;
  }

  [Benchmark(Description = "FastFloat.ParseDouble() - UTF8")]
  public double FastParserUtf8_()
  {
    double max = double.MinValue;

    foreach (byte[] l in _linesUtf8)
    {
      double d = FastDoubleParser.ParseDouble(l);
      max = d > max ? d : max;
    }
    return max;
  }

  [Benchmark(Description = "FastFloat.ParseDouble()")]
  public double FastParser_()
  {
    double max = double.MinValue;

    foreach (string l in _lines)
    {
      double d = FastDoubleParser.ParseDouble(l);
      max = d > max ? d : max;
    }
    return max;
  }





  [Benchmark(Description = "ParseNumberString() only")]
  public double FastParser_PNS()
  {
   double max = double.MinValue;

   foreach (string l in _lines)
   {
     unsafe { 
      
     fixed (char* p = l)
     {
       var pni = ParsedNumberString.ParseNumberString(p, p + l.Length);
       max = pni.exponent > max ? pni.exponent: max;
     }
      
      
      
     }
   }
   return max;
  }



  [Benchmark(Baseline = true, Description = "Double.Parse()")]
  public double Double_std()
  {
   double max = double.MinValue;
   foreach (string l in _lines)
   {
     double d = double.Parse(l, CultureInfo.InvariantCulture);

     max = d > max ? d : max;
   }
   return max;
  }


   [Params(@"data/canada.txt", @"data/mesh.txt", @"data/synthetic.txt")]
   public string FileName;

  [GlobalSetup]
  public void Setup()
  {
    Console.WriteLine("reading data");
    _lines = System.IO.File.ReadAllLines(FileName);
    _linesUtf8 = Array.ConvertAll(_lines, System.Text.Encoding.UTF8.GetBytes);
  }
}








public class Program
{
  public static void Main(string[] args)
  {

    	var config = DefaultConfig.Instance.WithSummaryStyle( SummaryStyle.Default.WithMaxParameterColumnWidth(100));
			BenchmarkRunner.Run<FFBencmark>(config);

  }
}
}