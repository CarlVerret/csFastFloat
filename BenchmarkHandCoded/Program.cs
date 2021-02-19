using BenchmarkHandCoded;
using System;
using System.Diagnostics;
using csFastFloat;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BenchmarkHandCoded
{
  internal class Program
  {
    public delegate double ParsingFunc(string[] values);

    static internal Tuple<double, double> time_it_ns<T>(string[] lines, ParsingFunc sut, long repeat)
    {
      double average = 0;
      double min_value = double.MaxValue;

      //  warmup
      for (int i = 0; i != 100; i++)
      {
        
        sut(lines);
      }


      Stopwatch sw = new Stopwatch();

      for (int i = 0; i != repeat; i++)
      {
        // ...; 

        sw.Restart();
        sut(lines);

        sw.Stop();
        var dif = sw.ElapsedMilliseconds * 1000000;

        average += dif;


        min_value = min_value < dif ? min_value : dif;
      }

      average /= repeat;
      return new Tuple<double, double>(min_value, average);
    }

    internal static string[] GetLinesFromFile(string fileName) =>
        System.IO.File.ReadAllLines(fileName);

    private static double find_max_fast_float(string[] lines)
    {
      double x;
      double answer = 0;
      foreach (string l in lines)
      {
        x = FastDoubleParser.ParseDouble(l);
        answer = answer > x ? answer : x;
      }
     
      return answer;
    }

  



    private static double find_max_double_parse(string[] lines)
    {
      double x;
      double answer = 0;
      foreach (string l in lines)
      {
        x = Double.Parse(l, CultureInfo.InvariantCulture);
        answer = answer > x ? answer : x;
      }

      return answer;
    }

    static private void pretty_print(double volume, uint number_of_floats, string name, Tuple<double, double> result)
    {
      double volumeMB = volume / (1024.0 * 1024.0);
      Console.Write("{0,-40}: {1,8:f2} MB/s (+/- {2:f1} %) ", name, volumeMB * 1000000000 / result.Item1, (result.Item2 - result.Item1) * 100.0 / result.Item2);
      Console.Write("{0,8:f2} Mfloat/s  ", number_of_floats * 1000 / result.Item1);
      Console.Write(" {0,8:f2} ns/f \n", (double)result.Item1 / number_of_floats);
    }

    private static void Main(string[] args)
    {
      string fileName = @"data/canada.txt";
      var lines = GetLinesFromFile(fileName);
      int volume = 0;
      foreach (string l in lines)
      {
        volume += l.Length;
      }


      Console.WriteLine("Canada.txt");
      Console.WriteLine("--------------------------");
      double volumeMB = volume / (1024.0 * 1024.0);
      Console.WriteLine($"Volume : {volumeMB}");

      process_test(lines, (double)volume);


      Console.WriteLine("");
      Console.WriteLine("");
 
      Console.WriteLine("Mesh.txt");
      Console.WriteLine("--------------------------");
 
       lines = GetLinesFromFile(@"data/mesh.txt");
       volume = 0;
      foreach (string l in lines)
      {
        volume += l.Length;
      }
       volumeMB = volume / (1024.0 * 1024.0);
      Console.WriteLine($"Volume : {volumeMB}");

      process_test(lines, (double)volume);





    }

    private static void process_test(string[] lines, double volume)
    {

      pretty_print(volume, (uint)lines.Length, "Double.Parse", time_it_ns<double>(lines, find_max_double_parse, 100));
      pretty_print(volume, (uint)lines.Length, "FastParser.ParseDouble", time_it_ns<double>(lines, find_max_fast_float, 100));
       
    }
  }
}