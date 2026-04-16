using BreakInfinity;
using static BreakInfinity.BigDouble;

namespace Extendables
{
    public static class Methods
    {
        public static string Notate(this BigDouble num, int dec = 2, int dec2 = 2)
        {
            var exponent = Truncate(Log10(Abs(num)));
            var mantissa = num / Pow(10, exponent);
            return num >= 1000 ? $"{mantissa.ToString($"N{dec2}")}e{exponent:N0}" : num.ToString($"N{dec}");
        }
    }
}
