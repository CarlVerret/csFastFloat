# csFastFloat : a fast and accurate float parser
[![.NET](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml)

C# port of Daniel Lemire's [fast_float](https://github.com/fastfloat/fast_float)  fully ported from C++ to C#. It is over 8 times faster than the standard library in some cases while providing exact results.  

# Benchmarks

We use the realistic files  in /data. The mesh.txt data file contains numbers that are easier to parse whereas the canada.txt data file is representative of a more challenging scenario. Synthetic.txt contains 150 000 random floats. We compare  the `Double.Parse()` function from the runtime library with our `FastFloat.ParseDouble()` function. The `ParseNumberString()` function parses the string itself without any float computation: it might represent an upper bound on the possible performance. 

By default, C# uses UTF-16 encoding for string reprsentation.  Our parser does support both UTF-16 and UTF-8 inputs.


``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04 (container)
AMD EPYC 7262, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.102
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0

|                              Method |           FileName |        Mean |     Error |    StdDev |         Min | Ratio | MFloat/s |     MB/s |
|------------------------------------ |------------------- |------------:|----------:|----------:|------------:|------:|---------:|---------:|
|                          Utf8Parser |    data/canada.txt | 29,366.8 us | 149.60 us | 132.62 us | 29,163.2 us |  0.82 |     3.81 |    71.60 |
|             FastFloat.ParseDouble() |    data/canada.txt |  4,925.2 us |  31.07 us |  29.07 us |  4,885.2 us |  0.14 |    22.75 |   427.42 |
|          FastFloat.TryParseDouble() |    data/canada.txt |  4,884.3 us |   5.75 us |   4.49 us |  4,878.3 us |  0.14 |    22.78 |   428.02 |
|    'FastFloat.ParseDouble() - UTF8' |    data/canada.txt |  4,507.4 us |  11.95 us |  11.18 us |  4,495.8 us |  0.13 |    24.72 |   464.44 |
| 'FastFloat.TryParseDouble() - UTF8' |    data/canada.txt |  4,849.9 us |  10.73 us |   9.51 us |  4,836.0 us |  0.14 |    22.98 |   431.76 |
|          'ParseNumberString() only' |    data/canada.txt |  2,244.4 us |   1.17 us |   1.04 us |  2,243.0 us |  0.06 |    49.54 |   930.89 |
|           'ParseNumberString UTF-8' |    data/canada.txt |  2,345.0 us |  27.19 us |  24.10 us |  2,323.2 us |  0.07 |    47.83 |   898.77 |
|                      Double.Parse() |    data/canada.txt | 35,670.7 us | 297.33 us | 278.12 us | 35,162.8 us |  1.00 |     3.16 |    59.38 |
|                                     |                    |             |           |           |             |       |          |          |
|                          Utf8Parser |      data/mesh.txt |  3,956.1 us |  29.18 us |  25.87 us |  3,918.5 us |  0.58 |    18.63 |   158.22 |
|             FastFloat.ParseDouble() |      data/mesh.txt |  1,846.0 us |   9.26 us |   8.67 us |  1,822.6 us |  0.27 |    40.06 |   340.18 |
|          FastFloat.TryParseDouble() |      data/mesh.txt |  1,933.1 us |   0.29 us |   0.22 us |  1,932.9 us |  0.28 |    37.78 |   320.77 |
|    'FastFloat.ParseDouble() - UTF8' |      data/mesh.txt |  1,547.8 us |   4.27 us |   3.33 us |  1,541.0 us |  0.23 |    47.39 |   402.35 |
| 'FastFloat.TryParseDouble() - UTF8' |      data/mesh.txt |  1,881.9 us |   0.36 us |   0.30 us |  1,881.3 us |  0.28 |    38.81 |   329.56 |
|          'ParseNumberString() only' |      data/mesh.txt |  1,179.5 us |   0.57 us |   0.50 us |  1,178.6 us |  0.17 |    61.95 |   526.03 |
|           'ParseNumberString UTF-8' |      data/mesh.txt |    920.8 us |   3.00 us |   2.81 us |    918.0 us |  0.13 |    79.54 |   675.35 |
|                      Double.Parse() |      data/mesh.txt |  6,821.5 us |  56.06 us |  52.44 us |  6,737.3 us |  1.00 |    10.84 |    92.02 |
|                                     |                    |             |           |           |             |       |          |          |
|                          Utf8Parser | data/synthetic.txt | 37,208.8 us | 111.09 us |  86.73 us | 37,079.2 us |  0.77 |     4.05 |    76.11 |
|             FastFloat.ParseDouble() | data/synthetic.txt |  5,217.9 us |   3.10 us |   2.75 us |  5,213.5 us |  0.11 |    28.77 |   541.29 |
|          FastFloat.TryParseDouble() | data/synthetic.txt |  5,267.5 us |  11.07 us |  10.35 us |  5,258.2 us |  0.11 |    28.53 |   536.68 |
|    'FastFloat.ParseDouble() - UTF8' | data/synthetic.txt |  5,461.4 us |   3.45 us |   2.88 us |  5,453.7 us |  0.11 |    27.50 |   517.44 |
| 'FastFloat.TryParseDouble() - UTF8' | data/synthetic.txt |  5,108.3 us |   6.62 us |   5.53 us |  5,100.7 us |  0.11 |    29.41 |   553.25 |
|          'ParseNumberString() only' | data/synthetic.txt |  2,543.6 us |   1.87 us |   1.75 us |  2,540.8 us |  0.05 |    59.04 |  1110.68 |
|           'ParseNumberString UTF-8' | data/synthetic.txt |  2,899.8 us |   4.77 us |   3.98 us |  2,896.0 us |  0.06 |    51.80 |   974.46 |
|                      Double.Parse() | data/synthetic.txt | 48,119.0 us | 404.24 us | 378.13 us | 47,748.5 us |  1.00 |     3.14 |    59.10 |

```

In this repo [FastFloatTestBench](https://github.com/CarlVerret/FastFloatTestBench) we demonstrate a concrete performance gain obtained with FastFloat.ParseDouble() with the [CSVHelper](https://github.com/JoshClose/CsvHelper) library.  This is one of the fastest CSV parser available.

Single and multiple columns files have been tested. :
- Canada.txt mesh.txt and Synthetic.txt are the same from previous benchmark.
- World cities population data (100k/300k) are real data obtained from [OpenDataSoft](https://public.opendatasoft.com/explore/dataset/worldcitiespop).

Benchmark is run on same environment.


``` ini
BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04 (container)
AMD EPYC 7262, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.102
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0
|                                Method |      fileName |      Mean |    Error |   StdDev |       Min | Ratio | MFloat/s |
|-------------------------------------- |-------------- |----------:|---------:|---------:|----------:|------:| --------:|
|          'Double.Parse() - singlecol' |    canada.txt |  85.74 ms | 0.361 ms | 0.320 ms |  85.08 ms |  1.00 |     1.31 |
| 'FastFloat.ParseDouble() - singlecol' |    canada.txt |  41.61 ms | 0.161 ms | 0.150 ms |  41.34 ms |  0.49 |     2.69 |
|                                       |               |           |          |          |           |       |          |
|          'Double.Parse() - singlecol' |      mesh.txt |   5.29 ms | 0.252 ms | 0.236 ms |  34.91 ms |  1.00 |     2.09 |
| 'FastFloat.ParseDouble() - singlecol' |      mesh.txt |   0.89 ms | 0.177 ms | 0.166 ms |  20.64 ms |  0.59 |     3.54 |
|                                       |               |           |          |          |           |       |          |
|          'Double.Parse() - singlecol' | synthetic.csv |   4.15 ms | 0.760 ms | 0.673 ms | 113.23 ms |  1.00 |     1.32 |
| 'FastFloat.ParseDouble() - singlecol' | synthetic.csv |   6.53 ms | 0.403 ms | 0.377 ms |  55.85 ms |  0.49 |     2.69 |
|                                       |               |           |          |          |           |       |          |
|           'Double.Parse() - multicol' |  w-c-100K.csv | 191.88 ms | 1.811 ms | 1.694 ms | 189.68 ms |  1.00 |     1.05 |
|        'FastFloat.Parse() - multicol' |  w-c-100K.csv | 171.18 ms | 1.386 ms | 1.082 ms | 168.70 ms |  0.89 |     1.19 |
|                                       |               |           |          |          |           |       |          |
|           'Double.Parse() - multicol' |  w-c-300K.csv | 587.42 ms | 2.435 ms | 2.277 ms | 582.98 ms |  1.00 |     1.03 |
|        'FastFloat.Parse() - multicol' |  w-c-300K.csv | 493.40 ms | 5.625 ms | 4.697 ms | 487.38 ms |  0.84 |     1.23 |

```



# Requirements

.NET standard 2.0 or newer. Under .NET 5 framework, the library takes advantage of the new Math.BigMul() & System.Runtime.Intrinsics.X86 SIMD functions.

# Compile and testing

As this library targets multiple framework, you can specify the target framework version with -f parameter :

``` command line
dotnet build -c Release -f net5.0
dotnet test -f net5.0

```
If you omit the target framework and you don't have both .net 5.0 and dotnetcore 3.1 SDKs installed you may experience an error when building or running tests.

The set of unit tests in /TestcsFastFloat project combines unit tests from many libraries.  It includes tests used by the Go Team.
Additionnal info on Nigel Tao's work can be found [here](https://nigeltao.github.io/blog/2020/eisel-lemire.html#testing).

Some unit tests are based on [Approvals.net library](https://github.com/approvals/ApprovalTests.Net).  They require a diff tool installed on your computer.  Tests will be automatically skiped if no diff tool is found.


# Usage

Two functions are available: `FastDoubleParser.ParseDouble` and `FastFloatParser.ParseFloat`. Since v3.0, TryParse pattern is supported for each function.

`String`, `char *`  and `ReadOnlySpan<char>` are supported inputs.

```C#
using csFastFloat;


double x;
float y;
double answer = 0;
foreach (string l in lines)
{
        x = FastDoubleParser.ParseDouble(l);
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
This library is the main project in my master's degree under the direction of Professor [Daniel Lemire](https://github.com/lemire) at TELUQ University.
A special thanks to [Egor Bogatov](https://github.com/EgorBo) and all contributors for their really meaningful contribution.

# Reference

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), [Software: Pratice and Experience](https://onlinelibrary.wiley.com/doi/10.1002/spe.2984) 51 (8), 2021.
