using System.Diagnostics;

namespace csFastFloat.Structures
{
  internal unsafe struct DigitsBuffer
  {
    private fixed byte digits[(int)Constants.max_digits];
    
    public byte this[int index]
    {
      get => this[(uint)index];
      set => this[(uint)index] = value;
    }
    
    public byte this[uint index]
    {
      get
      {
        Debug.Assert(index < Constants.max_digits);
        return digits[index];
      }
      
      set
      {
        Debug.Assert(index < Constants.max_digits);
        digits[index] = value;
      }
    }
  }
}
