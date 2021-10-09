using System;
using System.Collections.Generic;

namespace vTuneBench
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("debut");

      //var filenames = new List<string>() {  @"data/canada.txt", @"data/mesh.txt", @"data/synthetic.txt"};

      //foreach(string f in filenames)
      //{
      //  Console.WriteLine($"file : {f}");
      //  Console.WriteLine("opening and reading file");

      //  var _lines = System.IO.File.ReadAllLines(f);
      //  var  _linesUtf8 = Array.ConvertAll(_lines, System.Text.Encoding.UTF8.GetBytes);


      //  double max = double.MinValue;

      //  foreach (string l in _lines)
      //  {


      double value = csFastFloat.FastDoubleParserSIMD2.ParseDouble(System.Text.Encoding.UTF8.GetBytes( "0.0123456789"));
      Console.WriteLine($"valeur: {value} ");
      //    if (value > max) max = value;  

      //  }

      //  Console.WriteLine($"max : {max} " );
      //}

      Console.WriteLine("end");



    }
  }
}
