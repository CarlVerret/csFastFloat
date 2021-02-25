# csFastFloat : a fast and accurate float parser
[![.NET](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CarlVerret/csFastFloat/actions/workflows/dotnet.yml)

C# port of Daniel Lemire's [fast_float](https://github.com/fastfloat/fast_float)  fully ported from C++ to C#. It is almost 7 times faster than the standard library in some cases while providing exact results.



# Benchmarks

We use the realistic files  in /data. The mesh.txt data file contains numbers that are easier to parse whereas the canada.txt data file is representative of a more challenging scenario. We compare  the `Double.Parse()` function from the runtime library with our `FastFloat.ParseDouble()` function. The `ParseNumberString() only` function parses the string itself without any float computation: it might represent an upper bound on the possible performance.


``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04 (container)
AMD EPYC 7262, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.102
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0  

|                     Method |        FileName |      Mean |     Error |    StdDev |       Min | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated | MFloat/s |     MB/s |
|--------------------------- |---------------- |----------:|----------:|----------:|----------:|------:|------:|------:|------:|----------:|---------:|---------:|
|    FastFloat.ParseDouble() | data/canada.txt |  5.974 ms | 0.0060 ms | 0.0053 ms |  5.965 ms |  0.16 |     - |     - |     - |       2 B |    18.63 |   350.04 |
| 'ParseNumberString() only' | data/canada.txt |  3.082 ms | 0.0009 ms | 0.0008 ms |  3.081 ms |  0.08 |     - |     - |     - |       1 B |    36.07 |   677.80 |
|             Double.Parse() | data/canada.txt | 37.459 ms | 0.2417 ms | 0.2142 ms | 37.003 ms |  1.00 |     - |     - |     - |      21 B |     3.00 |    56.43 |
|                            |                 |           |           |           |           |       |       |       |       |           |          |          |
|    FastFloat.ParseDouble() |   data/mesh.txt |  1.815 ms | 0.0144 ms | 0.0134 ms |  1.798 ms |  0.26 |     - |     - |     - |       1 B |    40.61 |   344.84 |
| 'ParseNumberString() only' |   data/mesh.txt |  1.314 ms | 0.0021 ms | 0.0017 ms |  1.312 ms |  0.19 |     - |     - |     - |       1 B |    55.66 |   472.64 |
|             Double.Parse() |   data/mesh.txt |  6.911 ms | 0.1136 ms | 0.1062 ms |  6.758 ms |  1.00 |     - |     - |     - |       2 B |    10.81 |    91.75 |

```

In this repo [FastFloatTestBench](https://github.com/CarlVerret/FastFloatTestBench) we demonstrate a concrete performance gain obtained with FastFloat.ParseDouble() with the [CSVHelper](https://github.com/JoshClose/CsvHelper) library.  This is one of the fastest CSV parser available.  

Single and multiple columns files have been tested. :
- Canada.txt and mesh.txt are the same from previous benchmark.  
- Syntethic.csv is composed of 150 000 random floats. 
- World cities population data (100k/300k) are real data obtained from [OpenDataSoft](https://public.opendatasoft.com/explore/dataset/worldcitiespop).  

Benchmark is run on same environment.


``` ini
BenchmarkDotNet=v0.12.1, OS=ubuntu 20.04 (container)
AMD EPYC 7262, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.102
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0  
|                                Method |               fileName | fileSize | nbFloat |      Mean |    Error |   StdDev |       Min | Ratio | MFloat/s |
|-------------------------------------- |----------------------- |--------- |-------- |----------:|---------:|---------:|----------:|------:|---------:|
|          'Double.Parse() - singlecol' |    TestData/canada.txt |     2088 |  111126 |  84.46 ms | 0.271 ms | 0.226 ms |  84.16 ms |  1.00 |     1.32 |
|                  'Zeroes - singlecol' |    TestData/canada.txt |     2088 |  111126 |  33.59 ms | 0.214 ms | 0.178 ms |  33.21 ms |  0.40 |     3.35 |
| 'FastFloat.ParseDouble() - singlecol' |    TestData/canada.txt |     2088 |  111126 |  40.58 ms | 0.265 ms | 0.235 ms |  40.13 ms |  0.48 |     2.77 |
|                                       |                        |          |         |           |          |          |           |       |          |
|          'Double.Parse() - singlecol' |      TestData/mesh.txt |      691 |   73019 |  29.64 ms | 0.157 ms | 0.146 ms |  29.41 ms |  1.00 |     2.48 |
|                  'Zeroes - singlecol' |      TestData/mesh.txt |      691 |   73019 |  17.68 ms | 0.077 ms | 0.064 ms |  17.58 ms |  0.60 |     4.15 |
| 'FastFloat.ParseDouble() - singlecol' |      TestData/mesh.txt |      691 |   73019 |  20.06 ms | 0.188 ms | 0.176 ms |  19.82 ms |  0.68 |     3.68 |
|                                       |                        |          |         |           |          |          |           |       |          |
|          'Double.Parse() - singlecol' | TestData/synthetic.csv |     2969 |  150000 | 114.10 ms | 1.355 ms | 1.202 ms | 111.87 ms |  1.00 |     1.34 |
|                  'Zeroes - singlecol' | TestData/synthetic.csv |     2969 |  150000 |  46.48 ms | 0.197 ms | 0.184 ms |  46.20 ms |  0.41 |     3.25 |
| 'FastFloat.ParseDouble() - singlecol' | TestData/synthetic.csv |     2969 |  150000 |  54.29 ms | 0.683 ms | 0.605 ms |  53.40 ms |  0.48 |     2.81 |
|                                       |                        |          |         |           |          |          |           |       |          |
|           'Double.Parse() - multicol' |  TestData/w-c-100K.csv |     4842 |  200002 | 182.30 ms | 2.629 ms | 2.459 ms | 179.70 ms |  1.00 |     1.11 |
|                 'Zeroes() - multicol' |  TestData/w-c-100K.csv |     4842 |  200002 | 160.47 ms | 1.368 ms | 1.068 ms | 158.88 ms |  0.88 |     1.26 |
|        'FastFloat.Parse() - multicol' |  TestData/w-c-100K.csv |     4842 |  200002 | 168.60 ms | 1.217 ms | 1.079 ms | 166.84 ms |  0.92 |     1.20 |
|                                       |                        |          |         |           |          |          |           |       |          |
|           'Double.Parse() - multicol' |  TestData/w-c-300K.csv |    14526 |  600002 | 572.31 ms | 4.286 ms | 3.799 ms | 566.87 ms |  1.00 |     1.06 |
|                 'Zeroes() - multicol' |  TestData/w-c-300K.csv |    14526 |  600002 | 451.54 ms | 3.379 ms | 2.822 ms | 445.87 ms |  0.79 |     1.35 |
|        'FastFloat.Parse() - multicol' |  TestData/w-c-300K.csv |    14526 |  600002 | 479.76 ms | 3.103 ms | 2.423 ms | 477.05 ms |  0.84 |     1.26 |

```



# Requirements

.NET Core 3.1 or better. Under .NET 5 framework, the library takes advantage of the new Math.BigMul() function.  

# Usage

Two functions are available: `FastDoubleParser.ParseDouble` and `FastFloatParser.ParseFloat`.

`String` and `ReadOnlySpan<char>` are supported inputs.

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

For UTF-8 or ASCII inputs, you may pass a `ReadOnlySpan<byte>` argument.

# Testing

The set of unit tests in /TestcsFastFloat project combines unit tests from many libraries.  It includes tests used by the Go Team.  
Additionnal info on Nigel Tao's work can be found [here](https://nigeltao.github.io/blog/2020/eisel-lemire.html#testing).

# Credit
This library is the main project in my master's degree under the direction of Professor [Daniel Lemire](https://github.com/lemire) at TELUQ University.
A special thanks to [Egor Bogatov](https://github.com/EgorBo) for his really meaningful contribution.

# Reference

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), arXiv:2101.11408
