using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using csFastFloat;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;

[SimpleJob(RuntimeMoniker.NetCoreApp50)]
[Config(typeof(Config))]
[IterationCount(100)]
public class MyBencmark
{
  private string[] _lines;

  private class Config : ManualConfig
  {
    public Config()
    {
      AddColumn(
          StatisticColumn.Min);
    }
  }

  [Benchmark(Description = "FastFloat")]
  public void FastParser_()
  {
    foreach (string l in _lines)
      FastParser.ParseDouble(l);
  }

  [Benchmark(Baseline = true, Description = "Double.Parse()")]
  public void Double_std()
  {
    foreach (string l in _lines)
      Double.Parse(l, CultureInfo.InvariantCulture);
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