# csFastFloat : a fast and accurate float parser

C# port of Daniel Lemire's [fast_float](https://github.com/fastfloat/fast_float)  fully ported from C to C#. It is almost 7 times faster than the standard library.

# Benchmarks

Using content of files in /data : Canada.txt and mesh.txt

Comparing standard Double.Parse() vs FastFloat.ParseDouble().  
'ParseNumberString() only' parses the string itself without any float computation.


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


# Requirements

This library is optimized for .NET 5 framework, taking advantage of the new Math.BigMul() function.  
It does support .net core 3.1

# Usage

Two functions are available.  ParseDouble and ParseFloat.

```
C#
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

# Reference

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), arXiv:2101.11408
