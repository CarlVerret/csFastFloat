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
|    FastFloat.ParseDouble() | data/canada.txt |  5.140 ms | 0.0280 ms | 0.0262 ms |  5.105 ms |  0.14 |    21.77 |   408.99 |
| 'ParseNumberString() only' | data/canada.txt |  2.540 ms | 0.0053 ms | 0.0047 ms |  2.531 ms |  0.07 |    43.90 |   824.87 |
|             Double.Parse() | data/canada.txt | 37.147 ms | 0.3284 ms | 0.3071 ms | 36.443 ms |  1.00 |     3.05 |    57.29 |
|                            |                 |           |           |           |           |       |          |          |
|    FastFloat.ParseDouble() |   data/mesh.txt |  2.083 ms | 0.0029 ms | 0.0024 ms |  2.080 ms |  0.29 |    35.10 |   298.07 |
| 'ParseNumberString() only' |   data/mesh.txt |  1.298 ms | 0.0034 ms | 0.0032 ms |  1.294 ms |  0.18 |    56.45 |   479.30 |
|             Double.Parse() |   data/mesh.txt |  7.086 ms | 0.0911 ms | 0.0852 ms |  6.931 ms |  1.00 |    10.54 |    89.45 |

```

# Requirements

.NET Core 3.1 or better. Under .NET 5 framework, the library takes advantage of the new Math.BigMul() function.
Since v2.0, net standard 2.0 is supported. 

# Usage

Two functions are available: `FastDoubleParser.ParseDouble` and `FastFloatParser.ParseFloat`.  Since v3.0, TryParse pattern is supported for each function.

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

- Daniel Lemire, [Number Parsing at a Gigabyte per Second](https://arxiv.org/abs/2101.11408), arXiv:2101.11408
