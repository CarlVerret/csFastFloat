using csFastFloat;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace TestcsFastFloat.Tests.ff_suppl_tests
{
  public class SupplFilesTest : BaseTestClass
  {
    /// <summary>
    /// Verify FastParser for every .txt file of a given path
    /// </summary>
    /// <param name="args"></param>

    [Fact()]
    private void AllFiles()
    {
      //string pathValidation = args[0] ?? throw new ArgumentNullException("Search path");

      string pathValidation = @".\data_files";

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
          Assert.True(_f == f);
          double d = FastParser.ParseDouble(sut[3]);
          Assert.True(_d == d);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"parsing error on : {curntLine}. {ex.Message}");
        }
      }
      fs.Close();
    }
  }
}