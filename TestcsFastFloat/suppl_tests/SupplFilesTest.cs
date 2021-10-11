using csFastFloat;
using System;
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
    [Trait("Category", "Files Test")]
    [Fact()]
    private void AllFiles()
    {
      string pathValidation = "data_files";

      if (!Directory.Exists(pathValidation))
      {
        // Important: do not assume that path separator is \.
        pathValidation = "TestcsFastFloat" + Path.PathSeparator + pathValidation;
        if (!Directory.Exists(pathValidation))
        {
          Console.WriteLine("I looked for the data_files directory, and could not find it.");
          throw new ArgumentException("Invalid search path");
        }
      }

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



    [Trait("Category", "Files Test")]
    [Fact()]
    private void BasicFiles()
    {
      string pathValidation = "basic_files";

      if (!Directory.Exists(pathValidation))
      {
        // Important: do not assume that path separator is \.
        pathValidation = "TestcsFastFloat" + Path.PathSeparator + pathValidation;
        if (!Directory.Exists(pathValidation))
        {
          Console.WriteLine("I looked for the data_files directory, and could not find it.");
          throw new ArgumentException("Invalid search path");
        }
      }

      foreach (var fileName in Directory.GetFiles(pathValidation, "*.txt"))
      {
        Console.WriteLine(fileName);
        try
        {
          VerifyBasicFile(fileName);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
    }

    private static void VerifyBasicFile(string fileName)
    {

      var lines = System.IO.File.ReadAllLines(fileName);

      foreach (var l in lines)
      {
        //double res = FastDoubleParser.ParseDouble(l);
        if (FastDoubleParser.TryParseDouble(l, out double res))
          Assert.Equal(res, double.Parse(l));
        else
          throw new Exception($"Can't parse {l}");
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
      int counter = 0;
      int testCount = 0;
      while (!fs.EndOfStream)
      {
          string curntLine = fs.ReadLine();
          var sut = curntLine.Split();
          if (sut.Length != 4) throw new Exception($"Invalid file in file {curntLine}");

          // Make sense of s,f,d
          short _s = ShortFromHexString(sut[0]);
          float _f = FloatFromHexString(sut[1]);
          double _d = DoubleFromHexString(sut[2]);

          // parse and assert equality
          float f = FastFloatParser.ParseFloat(sut[3]);
          Assert.True(_f == f);
          double d = FastDoubleParser.ParseDouble(sut[3]);
          Assert.True(_d == d);

          // parse and assert equality
          float f_span = FastFloatParser.ParseFloat(sut[3].AsSpan());
          Assert.True(_f == f_span);
          double d_span = FastDoubleParser.ParseDouble(sut[3].AsSpan());
          Assert.True(_d == d_span);

          // parse and assert equality
          float f_utf8 = FastFloatParser.ParseFloat(System.Text.Encoding.UTF8.GetBytes(sut[3]));
          Assert.True(_f == f_utf8);
          double d_utf8 = FastDoubleParser.ParseDouble(System.Text.Encoding.UTF8.GetBytes(sut[3]));
          Assert.True(_d == d_utf8);

          counter++;
          testCount += 6;
      }
      fs.Close();
      Console.WriteLine($"processed {counter} numbers, {testCount} tests in total.");
    }
  }
}