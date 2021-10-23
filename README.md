# csFastFloat : a fast and accurate float parser
[![.NET](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml)

C# port of Daniel Lemire's [fast_float](https://github.com/fastfloat/fast_float)  fully ported from C++ to C#. It is up to 9 times faster than the standard library in some cases while providing exact results.



# Benchmarks

We use the realistic files  in /data. The mesh.txt data file contains numbers that are easier to parse whereas the canada.txt data file is representative of a more challenging scenario.  Synthetic.txt contains 150 000 random floats. We compare  the `Double.Parse()` function from the runtime library with our `FastFloat.ParseDouble()` function. The `ParseNumberString() only` function parses the string itself without any float computation: it might represent an upper bound on the possible performance.


``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04 (container)
AMD EPYC 7262, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.102
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0

|                         Method |           FileName |      Mean |       Min | Ratio | MFloat/s |     MB/s |
|------------------------------- |------------------- |----------:|----------:|------:|---------:|---------:|
|     FastFloat.TryParseDouble() |    data/canada.txt |  4.586 ms |  4.559 ms |  0.12 |    24.38 |   458.00 |
|     'ParseNumberString() only' |    data/canada.txt |  2.472 ms |  2.411 ms |  0.07 |    46.10 |   866.13 |
|                 Double.Parse() |    data/canada.txt | 37.537 ms | 37.159 ms |  1.00 |     2.99 |    56.19 |
|                                |                    |           |           |       |          |          |
|     FastFloat.TryParseDouble() |      data/mesh.txt |  1.834 ms |  1.834 ms |  0.27 |    39.81 |   338.05 |
|     'ParseNumberString() only' |      data/mesh.txt |  1.168 ms |  1.164 ms |  0.17 |    62.71 |   532.43 |
|                 Double.Parse() |      data/mesh.txt |  6.850 ms |  6.788 ms |  1.00 |    10.76 |    91.34 |
|                                |                    |           |           |       |          |          |
|     FastFloat.TryParseDouble() | data/synthetic.txt |  5.147 ms |  5.118 ms |  0.11 |    29.31 |   551.44 |
|     'ParseNumberString() only' | data/synthetic.txt |  2.655 ms |  2.653 ms |  0.05 |    56.54 |  1063.78 |
|                 Double.Parse() | data/synthetic.txt | 48.744 ms | 48.283 ms |  1.00 |     3.11 |    58.45 |

```

In this repo [FastFloatTestBench](https://github.com/CarlVerret/FastFloatTestBench) we demonstrate a concrete performance gain obtained with FastFloat.ParseDouble() with the [CSVHelper](https://github.com/JoshClose/CsvHelper) library.  This is one of the fastest CSV parser available.



# Requirements

.NET Standard 2.0 or newer. Under .NET 5 framework, the library takes advantage of the new Math.BigMul() and Sse4.1 (SIMD) functions.

# Compile and testing

As this library targets multiple framework, you can specify the target framework version with -f parameter :

``` command line
dotnet build -c Release -f net5.0
dotnet test -f net5.0

```
If you omit the target framework and you don't have both .net 5.0 and dotnetcore 3.1 SDKs installed you may experience an error when building or running tests.

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
This library is the main project in my master's degree under the direction of Professor [Daniel Lemire](https://github.com/lemire) at TELUQ University.
A special thanks to [Egor Bogatov](https://github.com/EgorBo) and all contributors for their really meaningful contribution.

# Reference

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), [Software: Pratice and Experience](https://onlinelibrary.wiley.com/doi/10.1002/spe.2984) 51 (8), 2021.
