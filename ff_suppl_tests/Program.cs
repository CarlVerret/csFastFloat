using csFastFloat;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;

namespace ff_suppl_tests
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      //const std::string input = ;

      //var d = FastParser.ParseDouble("2.22507385850720138309e-308");

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

          float _f = FloatFromHexString(sut[1]);
          float f = FastParser.ParseFloat(sut[3]);

          double _d = DoubleFromHexString(sut[2]);
          double d = FastParser.ParseDouble(sut[3]);
          Debug.Assert(_d == d);
        }
        catch (Exception ex)
        {
          Console.WriteLine("erreur : " + curntLine);
        }
      }
      fs.Close();
    }

    //bool check_file(std::string file_name)
    //{
    //  bool is_ok{ true};
    //  std::cout << "Checking " << file_name << std::endl;
    //  size_t number{ 0};
    //  std::fstream newfile(file_name, std::ios::in);
    //  if (newfile.is_open())
    //  {
    //    std::string str;
    //    while (std::getline(newfile, str))
    //    {
    //      if (str.size() > 0)
    //      {
    //        uint16_t float16;
    //        uint32_t float32;
    //        uint64_t float64;
    //        auto r16 =
    //            std::from_chars(str.data(), str.data() + str.size(), float16, 16);
    //        if (r16.ptr == str.data())
    //        {
    //          std::cerr << "(16) couldn't make sense of " << str << std::endl;
    //          break;
    //        }
    //        auto r32 = std::from_chars(str.data() + 5, str.data() + str.size(),
    //                                   float32, 16);
    //        if (r16.ptr == r32.ptr)
    //        {
    //          std::cerr << "(32) couldn't make sense of " << str << std::endl;
    //          break;
    //        }
    //        auto r64 = std::from_chars(str.data() + 14, str.data() + str.size(),
    //                                   float64, 16);
    //        if (r64.ptr == r32.ptr)
    //        {
    //          std::cerr << "(64) couldn't make sense of " << str << std::endl;
    //          break;
    //        }
    //        float float32_exact;
    //        double float64_exact;
    //        ::memcpy(&float32_exact, &float32, sizeof(float32));
    //        ::memcpy(&float64_exact, &float64, sizeof(float64));
    //        const char* a = str.data() + 31;
    //        const char* b = str.data() + str.size();
    //        float p32;
    //        double p64;
    //        fast_float::from_chars(a, b, p32);
    //        fast_float::from_chars(a, b, p64);
    //        uint32_t float32_parsed;
    //        uint64_t float64_parsed;
    //        ::memcpy(&float32_parsed, &p32, sizeof(p32));
    //        ::memcpy(&float64_parsed, &p64, sizeof(p64));
    //        if (float32_parsed != float32)
    //        {
    //          std::cout << "bad 32 " << str << std::endl;
    //          std::cout << std::hex << float32 << std::endl;
    //          std::cout << " parsed " << std::hexfloat << p32 << " ("
    //                    << std::defaultfloat << p32 << ")" << std::endl;
    //          std::cout << " expected " << std::hexfloat << float32_exact << " ("
    //                    << std::defaultfloat << float32_exact << ")" << std::endl;

    //          is_ok = false;
    //        }
    //        if (float64_parsed != float64)
    //        {
    //          std::cout << "bad 64 " << str << std::endl;
    //          std::cout << std::hex << float64 << std::endl;
    //          std::cout << " parsed " << std::hexfloat << p64 << " ("
    //                    << std::defaultfloat << p64 << ")" << std::endl;
    //          std::cout << " expected " << std::hexfloat << float64_exact << " ("
    //                    << std::defaultfloat << float64_exact << ")" << std::endl;
    //          is_ok = false;
    //        }
    //        number++;
    //      }
    //    }
    //    std::cout << "checked " << std::defaultfloat << number << " values" << std::endl;
    //    newfile.close(); // close the file object
    //  }
    //  else
    //  {
    //    std::cout << "Could not read  " << file_name << std::endl;
    //    return false;
    //  }
    //  return is_ok;
    //}

    //    // return true on success
    //    bool validate(const char* dirname)
    //{
    //  size_t total_count = 0;
    //    const char* extension = ".txt";
    //    size_t dirlen = std::strlen(dirname);
    //    struct dirent **entry_list;
    //int c = scandir(dirname, &entry_list, 0, alphasort);
    //if (c< 0)
    //{
    //  printf("error accessing %s \n", dirname);
    //  return false;
    //}
    //if (c == 0)
    //{
    //  printf("nothing in dir %s \n", dirname);
    //  return false;
    //}
    //bool error = false;
    //bool needsep = (strlen(dirname) > 1) && (dirname[strlen(dirname) - 1] != '/');
    //for (int i = 0; i < c; i++)
    //{
    //  const char* name = entry_list[i]->d_name;
    //  if (has_extension(name, extension) && (::strncmp("CMake", name, 5) != 0))
    //  {
    //    size_t filelen = std::strlen(name);
    //    char* fullpath = (char*)malloc(dirlen + filelen + 1 + 1);
    //    strcpy(fullpath, dirname);
    //    if (needsep)
    //    {
    //      fullpath[dirlen] = '/';
    //      strcpy(fullpath + dirlen + 1, name);
    //    }
    //    else
    //    {
    //      strcpy(fullpath + dirlen, name);
    //    }
    //    if (!check_file(fullpath))
    //    {
    //      error = true;
    //    }
    //    free(fullpath);
    //  }
    //}
    //for (int i = 0; i < c; ++i)
    //  free(entry_list[i]);
    //free(entry_list);
    //return !error;
    //}
  }
}