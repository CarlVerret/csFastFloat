using csFastFloat;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;

namespace ff_suppl_tests
{
  internal class Program
  {
    /// <summary>
    /// Verify FastParser for every .txt file of a given path
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
      string pathValidation = args[0] ?? throw new ArgumentNullException("Search path");

      if (!Directory.Exists(pathValidation))
        throw new ArgumentException("Invalid search path");

      foreach (var fileName in Directory.GetFiles(pathValidation, "*.txt"))
      {
        Console.WriteLine(fileName);
        try
        {
          VerifyFile(fileName);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
    }

    /// @Credit Dmitry Bychenko
    /// https://stackoverflow.com/questions/35449339/c-sharp-converting-from-float-to-hexstring-via-ieee-754-and-back-to-float/35450540#35450540
    private static short ShortFromHexString(string s)
    {
      var i = Convert.ToInt16(s, 16);
      var bytes = BitConverter.GetBytes(i);
      return BitConverter.ToInt16(bytes, 0);
    }

    private static float FloatFromHexString(string s)
    {
      var i = Convert.ToInt32(s, 16);
      var bytes = BitConverter.GetBytes(i);
      return BitConverter.ToSingle(bytes, 0);
    }

    private static double DoubleFromHexString(string s)
    {
      var i = Convert.ToInt64(s, 16);
      var bytes = BitConverter.GetBytes(i);
      return BitConverter.ToDouble(bytes, 0);
    }

    /// <summary>
    /// Check every line of the file which are required to be the following format
    /// 0000 00000000 0000000000000000 0e3
    /// ssss ffffffff dddddddddddddddd $$$$.....
    /// short float double string
    /// </summary>
    /// <param name="fileName">file to test</param>
    private static void VerifyFile(string fileName)
    {
      var fs = System.IO.File.OpenText(fileName);
      while (!fs.EndOfStream)
      {
        string curntLine = fs.ReadLine();

        try
        {
          var sut = curntLine.Split();
          if (sut.Length != 4) throw new Exception($"Invalid file in file {curntLine}");

          // Make sense of s,f,d
          short _s = ShortFromHexString(sut[0]);
          float _f = FloatFromHexString(sut[1]);
          double _d = DoubleFromHexString(sut[2]);

          // parse and assert equality
          float f = FastParser.ParseFloat(sut[3]);
          Debug.Assert(_f == f);
          double d = FastParser.ParseDouble(sut[3]);
          Debug.Assert(_d == d);
        }
        catch (Exception ex)
        {
          Console.WriteLine("parsing error on : " + curntLine);
        }
      }
      fs.Close();
    }
  }
}