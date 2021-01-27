using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using csFastFloat;
using System;
using System.Net.Sockets;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;

[SimpleJob(RuntimeMoniker.NetCoreApp50, baseline: true)]
//[SimpleJob(RuntimeMoniker.NetCoreApp30)]

public class MyBencmark
{
  [Params(
"-6,2681853871323554E-111",
"-4,1254613816663524E+142",
"2,1979705823851228E+283",
"-3,8953787906549024E+202",
"5,829924403438908E-230",
"-1,492320230997875E+236",
"-8,855392559521054E+113",
"2,7666148077878947E+176",
"8,606854783603359E-209",
"-4,133556142676581E-293",
"8,240101013037746E+105",
"-2,2133047000297085E-43",
"-1,7110288716219485E+194",
"1,5059980740922474E-43",
"-4,050284583005781E+89",
"-5,011591536014344E+301",
"1,7546557944036296E-223",
"-5,515637514590323E-121",
"1,4109283054779472E+79",
"3,738367452924929E+189",
"-7,015688725610912E+298",
"1,4593253032735424E-273",
"1,9375826602622584E+48",
"5,2587433574549136E+163",
"4,412647195014745E+224",
"1,4238196323794532E-209",
"-5,558391286283255E+145",
"3,867633733663355E+176",
"-2,770271609243207E+75",
"-4,4419514472655274E+79")]
  public string sut;

  [Benchmark]
  public void FastParser_() => FastParser.ParseDouble(sut);

  [Benchmark]
  public void Double_std() => Double.Parse(sut);

  public class Program
  {
    public static void Main(string[] args)
    {
      var a = 1;

      //  var d = double.Parse("- 1,8607245283054342363818436991534856973992070520151142825984000000e+58");

      var summary = BenchmarkRunner.Run<MyBencmark>();
    }
  }
}