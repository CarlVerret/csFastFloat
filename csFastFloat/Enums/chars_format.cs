using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestcsFastFloat")]

namespace csFastFloat.Enums
{
  [Flags]
  public enum chars_format
  {
    is_scientific = 0x1,
    is_fixed = 0x2,
    is_hex = 0x4,
    is_general = is_fixed | is_scientific
  };
}