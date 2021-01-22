namespace csFastFloat
{
  internal interface IBinaryFormat<T>
  {
    T NaN();

    T PositiveInfinity();

    T NegativeInfinity();

    int mantissa_explicit_bits();

    int minimum_exponent();

    int infinite_power();

    int sign_index();

    int min_exponent_fast_path();

    int max_exponent_fast_path();

    int max_exponent_round_to_even();

    int min_exponent_round_to_even();

    ulong max_mantissa_fast_path();

    int largest_power_of_ten();

    int smallest_power_of_ten();

    T exact_power_of_ten(long power);
  };
}