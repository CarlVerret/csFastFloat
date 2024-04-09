using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]
[assembly: InternalsVisibleTo("Benchmark")]

#if NET5_0_OR_GREATER
[module: SkipLocalsInit]
#endif
