using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

//using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using csFastFloat;
using System;
using System.Globalization;

[SimpleJob(RuntimeMoniker.NetCoreApp50)]
[Config(typeof(Config))]
//[IterationCount(100)] let BemchmarkDotNet determine how many iteration are required
public class MyBencmark
{
  private string[] _lines;

  private class Config : ManualConfig
  {
    public Config()
    {
      AddColumn(
          StatisticColumn.Min);
      // Todo : add MB/s + MFloat/s stats columns
    }
  }

  [Benchmark(Description = "FastFloat.PaseDouble()")]
  public double FastParser_()
  {
    double max = double.MinValue;

    foreach (string l in _lines)
    {
      double d = FastParser.ParseDouble(l);
      max = d > max ? d : max;
    }
    return max;
  }

  [Benchmark(Baseline = true, Description = "Double.Parse()")]
  public double Double_std()
  {
    double max = double.MinValue;
    foreach (string l in _lines)
    {
      double d = Double.Parse(l, CultureInfo.InvariantCulture);

      max = d > max ? d : max;
    }
    return max;
  }

  [GlobalSetup]
  public void Setup()
  {
    Console.WriteLine("reading data");
    string fileName = @"D:\TELUQ\Maitrise\fastfloat\simple_fastfloat_benchmark\data\canada.txt";
    _lines = System.IO.File.ReadAllLines(fileName);
  }
}

public class Program
{
  public static void Main(string[] args)
  {
    var summary = BenchmarkRunner.Run<MyBencmark>();
  }
}