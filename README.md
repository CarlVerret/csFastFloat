# csFastFloat : a fast and accurate float parser
[![.NET](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml)

C# port of Daniel Lemire's [fast_float](https://github.com/fastfloat/fast_float) fully ported from C++ to C#. In older versions (5.0 and under) it is up to 9 times faster than the standard library in some cases while providing exact results. An important portion of our work had been merged to .NET Runtime within version 7.0 of the .NET Framework. Yet the csFastFloat library remains substantially faster (e.g., 3 or 4 times faster) than the .NET Runtime in some instances.

The csFastFloat library is used by [Sep](https://github.com/nietras/Sep) which might be
the  World's Fastest .NET CSV Parser.

# Benchmarks

We use the realistic files  in /data. The mesh.txt data file contains numbers that are easier to parse whereas the canada.txt data file is representative of a more challenging scenario.  Synthetic.txt contains 150 000 random floats. We compare  the `Double.Parse()` function from the runtime library with our `FastFloat.ParseDouble()` function. The `ParseNumberString() only` function parses the string itself without any float computation: it might represent an upper bound on the possible performance.
Most recent benchmark done with .NET 9.0

``` ini

BenchmarkDotNet v0.14.0, Rocky Linux 9.4 (Blue Onyx)
Intel Xeon Gold 6338 CPU 2.00GHz, 2 CPU, 128 logical and 64 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=.NET 9.0  Runtime=.NET 9.0  

| Method                              | FileName           | Mean        | Min         | MFloat/s | MB/s     |
|------------------------------------ |------------------- |------------:|------------:|---------:|---------:|
| FastFloat.TryParseDouble()          | data/canada.txt    |  2,618.8 us |  2,614.8 us |    42.50 |   798.54 |
| Double.Parse()                      | data/canada.txt    | 11,216.7 us | 11,191.3 us |     9.93 |   186.57 |
|                                     |                    |             |             |          |          |
| FastFloat.TryParseDouble()          | data/mesh.txt      |    957.3 us |    949.3 us |    76.92 |   653.13 |
| Double.Parse()                      | data/mesh.txt      |  4,016.3 us |  4,005.7 us |    18.23 |   154.78 |
|                                     |                    |             |             |          |          |
| FastFloat.TryParseDouble()          | data/synthetic.txt |  3,141.1 us |  3,136.1 us |    47.83 |   899.86 |
| Double.Parse()                      | data/synthetic.txt | 14,575.3 us | 14,541.5 us |    10.32 |   194.07 |

```

In this repo [FastFloatTestBench](https://github.com/CarlVerret/FastFloatTestBench) we demonstrate a concrete performance gain obtained with FastFloat.ParseDouble() with the [CSVHelper](https://github.com/JoshClose/CsvHelper) library.  This is one of the fastest CSV parser available.



# Requirements

.NET Standard 2.0 or newer. Under .NET 5 framework or newer, the library takes advantage of the new Math.BigMul() and Sse4.1 (SIMD) functions.

# Compile and testing

As this library targets multiple framework, you can specify the target framework version with -f parameter :

``` command line
dotnet build -c Release -f net9.0
dotnet test -f net9.0

```
If you omit the target framework and you don't have both .net 8.0 and dotnetcore 3.1 SDKs installed you may experience an error when building or running tests.

The set of unit tests in /TestcsFastFloat project combines unit tests from many libraries.  It includes tests used by the Go Team.
Additional info on Nigel Tao's work can be found [here](https://nigeltao.github.io/blog/2020/eisel-lemire.html#testing).  It also include strtod test file that can be found [here](https://github.com/ahrvoje/numerics/blob/master/strtod/strtod_tests.toml).




Some unit tests are based on [Approvals.net library](https://github.com/approvals/ApprovalTests.Net).  They require a diff tool installed on your computer.  Tests will be automatically skiped if no diff tool is found.


# Usage

Two functions are available: `FastDoubleParser.ParseDouble` and `FastFloatParser.ParseFloat`. Since v3.0, TryParse pattern is supported for each function.

`String`, `char *`  and `ReadOnlySpan<char>` are supported inputs.

```C#
using csFastFloat;


double x;
float y;
double z;
double answer = 0;
foreach (string l in lines)
{
        x = FastDoubleParser.ParseDouble(l);
        FastDoubleParser.TryParseDouble(l, out z);
        
        y = FastFloatParser.ParseFloat(l);
}
```

Input strings are expected to be valid UTF-16.

Trailing content in the string is ignored.  You may pass an optional `out int characters_consumed` parameter
`FastDoubleParser.ParseDouble(l, out int characters_consumed)` if you wich to check how many characters were processed. Some users may want to fail when the number of characters consumed does not match the string length.


For UTF-8 or ASCII inputs, you may pass a `ReadOnlySpan<byte>` argument. You can also pass
an optional `out int characters_consumed` parameter to track the number of characters consumed
by the number pattern.




# Credit
This library was the main project in my master's degree under the direction of Professor [Daniel Lemire](https://github.com/lemire) at TELUQ University.
A special thanks to [Egor Bogatov](https://github.com/EgorBo) and all contributors for their really meaningful contribution.

# Reference

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), [Software: Practice and Experience](https://onlinelibrary.wiley.com/doi/10.1002/spe.2984) 51 (8), 2021.
