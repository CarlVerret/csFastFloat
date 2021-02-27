using System;

namespace csFastFloat
{
  internal partial class Utf8Literals
  {
    [StringLiteral.Utf8("infinity")]
    public static partial ReadOnlySpan<byte> infinity_string();
    
    [StringLiteral.Utf8("inf")]
    public static partial ReadOnlySpan<byte> inf_string();
    
    [StringLiteral.Utf8("+inf")]
    public static partial ReadOnlySpan<byte> pinf_string();
    
    [StringLiteral.Utf8("-inf")]
    public static partial ReadOnlySpan<byte> minf_string();
    
    [StringLiteral.Utf8("nan")]
    public static partial ReadOnlySpan<byte> nan_string();
    
    [StringLiteral.Utf8("+nan")]
    public static partial ReadOnlySpan<byte> pnan_string();
    
    [StringLiteral.Utf8("-nan")]
    public static partial ReadOnlySpan<byte> mnan_string();
  }
}
