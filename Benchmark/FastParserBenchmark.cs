using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using csFastFloat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;

[SimpleJob(RuntimeMoniker.NetCoreApp50, baseline: true)]
public class MyBencmark
{
  [Benchmark]
  [ArgumentsSource(nameof(Data))]
  public void FastParser_(string[] sut)
  {
    foreach (string s in sut)
    {
      FastParser.ParseDouble(s);
    }
  }

  [Benchmark]
  [ArgumentsSource(nameof(Data))]
  public void Double_std(string[] sut)
  {
    foreach (string s in sut)
    {
      Double.Parse(s, CultureInfo.InvariantCulture);
    }
  }

  public string[] Data()
  {
    Console.WriteLine("reading data");
    string fileName = @"D:\TELUQ\Maitrise\fastfloat\simple_fastfloat_benchmark\data\canada.txt";
    return System.IO.File.ReadAllLines(fileName);
  }

  public class Program
  {
    public static void Main(string[] args)
    {
      var summary = BenchmarkRunner.Run<MyBencmark>();
    }
  }
}