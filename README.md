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

|                     Method |        FileName |      Mean |     Error |    StdDev |       Min | Ratio | MFloat/s |     MB/s |
|--------------------------- |---------------- |----------:|----------:|----------:|----------:|------:|---------:|---------:|
|    FastFloat.ParseDouble() | data/canada.txt |  5.591 ms | 0.0053 ms | 0.0050 ms |  5.578 ms |  0.15 |    19.92 |   363.51 |
| 'ParseNumberString() only' | data/canada.txt |  3.105 ms | 0.0047 ms | 0.0044 ms |  3.098 ms |  0.08 |    35.87 |   654.58 |
|             Double.Parse() | data/canada.txt | 38.531 ms | 0.3308 ms | 0.3095 ms | 37.878 ms |  1.00 |     2.93 |    53.53 |
|                            |                 |           |           |           |           |       |          |          |
|    FastFloat.ParseDouble() |   data/mesh.txt |  2.056 ms | 0.0061 ms | 0.0054 ms |  2.046 ms |  0.30 |    35.68 |   274.64 |
| 'ParseNumberString() only' |   data/mesh.txt |  1.312 ms | 0.0006 ms | 0.0005 ms |  1.311 ms |  0.19 |    55.69 |   428.69 |
|             Double.Parse() |   data/mesh.txt |  6.855 ms | 0.1183 ms | 0.1106 ms |  6.689 ms |  1.00 |    10.92 |    84.03 |

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
|                                Method |               fileName | fileSize | nbFloat |      Mean |    Error |   StdDev |       Min | Ratio | RatioSD | MFloat/s |
|-------------------------------------- |----------------------- |--------- |-------- |----------:|---------:|---------:|----------:|------:|--------:|---------:|
|          'Double.Parse() - singlecol' |    TestData/canada.txt |     1980 |  111126 |  84.03 ms | 0.410 ms | 0.383 ms |  83.40 ms |  1.00 |    0.00 |     1.33 |
|                  'Zeroes - singlecol' |    TestData/canada.txt |     1980 |  111126 |  33.47 ms | 0.263 ms | 0.233 ms |  33.08 ms |  0.40 |    0.00 |     3.36 |
| 'FastFloat.ParseDouble() - singlecol' |    TestData/canada.txt |     1980 |  111126 |  40.69 ms | 0.249 ms | 0.233 ms |  40.34 ms |  0.48 |    0.00 |     2.75 |
|                                       |                        |          |         |           |          |          |           |       |         |          |
|          'Double.Parse() - singlecol' |      TestData/mesh.txt |      548 |   73019 |  30.29 ms | 0.123 ms | 0.109 ms |  30.09 ms |  1.00 |    0.00 |     2.43 |
|                  'Zeroes - singlecol' |      TestData/mesh.txt |      548 |   73019 |  18.06 ms | 0.139 ms | 0.123 ms |  17.85 ms |  0.60 |    0.00 |     4.09 |
| 'FastFloat.ParseDouble() - singlecol' |      TestData/mesh.txt |      548 |   73019 |  20.23 ms | 0.202 ms | 0.189 ms |  20.00 ms |  0.67 |    0.01 |     3.65 |
|                                       |                        |          |         |           |          |          |           |       |         |          |
|          'Double.Parse() - singlecol' | TestData/synthetic.csv |     2676 |  150000 | 111.97 ms | 1.082 ms | 0.959 ms | 110.85 ms |  1.00 |    0.00 |     1.35 |
|                  'Zeroes - singlecol' | TestData/synthetic.csv |     2676 |  150000 |  46.24 ms | 0.441 ms | 0.368 ms |  45.72 ms |  0.41 |    0.01 |     3.28 |
| 'FastFloat.ParseDouble() - singlecol' | TestData/synthetic.csv |     2676 |  150000 |  54.78 ms | 0.412 ms | 0.344 ms |  53.95 ms |  0.49 |    0.00 |     2.78 |
|                                       |                        |          |         |           |          |          |           |       |         |          |
|           'Double.Parse() - multicol' |  TestData/w-c-100K.csv |     4740 |  200002 | 184.84 ms | 1.988 ms | 1.859 ms | 182.46 ms |  1.00 |    0.00 |     1.10 |
|                 'Zeroes() - multicol' |  TestData/w-c-100K.csv |     4740 |  200002 | 157.53 ms | 2.785 ms | 2.605 ms | 154.81 ms |  0.85 |    0.02 |     1.29 |
|        'FastFloat.Parse() - multicol' |  TestData/w-c-100K.csv |     4740 |  200002 | 170.56 ms | 1.409 ms | 1.318 ms | 167.86 ms |  0.92 |    0.01 |     1.19 |
|                                       |                        |          |         |           |          |          |           |       |         |          |
|           'Double.Parse() - multicol' |  TestData/w-c-300K.csv |    14219 |  600002 | 582.92 ms | 3.237 ms | 2.703 ms | 577.78 ms |  1.00 |    0.00 |     1.04 |
|                 'Zeroes() - multicol' |  TestData/w-c-300K.csv |    14219 |  600002 | 450.91 ms | 2.095 ms | 1.636 ms | 448.44 ms |  0.77 |    0.00 |     1.34 |
|        'FastFloat.Parse() - multicol' |  TestData/w-c-300K.csv |    14219 |  600002 | 494.72 ms | 9.552 ms | 9.809 ms | 479.90 ms |  0.85 |    0.02 |     1.25 |

```



# Requirements

.NET Core 3.1 or better. Under .NET 5 framework, the library takes advantage of the new Math.BigMul() function.  

# Usage

Two functions are available.  ParseDouble and ParseFloat.

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

# Testing

The set of unit tests in /TestcsFastFloat project combines unit tests from many libraries.  It includes tests used by the Go Team.  
Additionnal info on Nigel Tao's work can be found [here](https://nigeltao.github.io/blog/2020/eisel-lemire.html#testing).

# Reference

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), arXiv:2101.11408
