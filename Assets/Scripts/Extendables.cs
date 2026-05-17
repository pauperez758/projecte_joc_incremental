using BreakInfinity;
using static BreakInfinity.BigDouble;

namespace Extendables
{
    public static class Methods
    {
        public static string Notate(this BigDouble num, int dec = 2, int dec2 = 2)
        {
            //var exponent = Truncate(Log10(Abs(num)));
            //var mantissa = num / Pow(10, exponent);
            //return num >= 1000 ? $"{mantissa.ToString($"N{dec2}")}e{exponent:N0}" : num.ToString($"N{dec}");

            // He canviat la notació científica bàsica per una notació amb sufixos fins arribar a 10*^33
            if (num < 1000) return num.ToString($"N{dec}");

            string[] suffixes = {
                "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc"
            };

            var exponent = (int)Truncate(Log10(Abs(num))).ToDouble();
            int suffixIndex = (exponent - 3) / 3;

            if (suffixIndex < suffixes.Length)
            {
                BigDouble divisor = Pow(10, suffixIndex * 3 + 3);
                BigDouble mantissa = num / divisor;
                return $"{mantissa.ToString($"N{dec2}")}{suffixes[suffixIndex]}";
            }
            else
            {
                BigDouble mantissa = num / Pow(10, exponent);
                return $"{mantissa.ToString($"N{dec2}")}e{exponent:N0}";
            }
        }

        public static string ToTimeFormat(this BigDouble seconds)
        {
            if (seconds <= 0 || IsNaN(seconds)) return "00:00:00";

            BigDouble d = Floor(seconds / 86400);
            BigDouble remainingSeconds = seconds - (d * 86400);

            BigDouble h = Floor(remainingSeconds / 3600);
            remainingSeconds = remainingSeconds - (h * 3600);

            BigDouble m = Floor(remainingSeconds / 60);
            BigDouble s = Floor(remainingSeconds - (m * 60));

            if (d > 0)
            {
                return $"{d:N0}d {h.ToDouble():00}:{m.ToDouble():00}:{s.ToDouble():00}";
            }

            return $"{h.ToDouble():00}:{m.ToDouble():00}:{s.ToDouble():00}";
        }
    }
}
