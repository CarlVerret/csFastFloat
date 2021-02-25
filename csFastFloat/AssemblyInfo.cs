using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]
[assembly: InternalsVisibleTo("Benchmark")]

#if NET5_0
[module: SkipLocalsInit]
#endif
