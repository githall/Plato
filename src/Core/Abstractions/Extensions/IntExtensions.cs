using System;

namespace PlatoCore.Abstractions.Extensions
{

    public static class IntExtensions
    {

        public static string ToPrettyInt(this int? input, bool precise = false)
        {
            if (input == null)
            {
                return default(int).ToString();
            }

            return input.Value.ToPrettyInt(precise);
        }

        public static string ToPrettyInt(this int input, bool precise = false)
        {

            var output = string.Empty;
            if (input == 0)
            {
                output = "0";
            }
            else
            {
                if (precise)
                {
                    output = input.ToString("n0");
                }
                else
                {
                    if (input > 1000000)
                    {
                        output = Math.Floor(Decimal.Divide(input, 1000000)).ToString() + "M";
                    }
                    else if (input > 100000)
                    {
                        output = Math.Floor(Decimal.Divide(input, 1000)).ToString("000.0") + "K";
                    }
                    else if (input > 10000)
                    {
                        output = Math.Floor(Decimal.Divide(input, 1000)).ToString("00.0") + "K";
                    }
                    else if (input > 1000)
                    {
                        output = Decimal.Divide(input, 1000).ToString("0.0") + "K";
                    }
                    else
                    {
                        output = input.ToString("n0");
                    }
                }
            }

            return output.Replace(".0K", "K");

        }

        public static int ToSafeCeilingDivision(this int input, int divideBy)
        {

            if (input <= 0)
            {
                return default(int);
            }

            if (divideBy <= 0)
            {
                return input;
            }

            return (int)System.Math.Ceiling(decimal.Divide(input, divideBy));

        }

        public static double ToSafeDivision(this int input, int total)
        {

            if (input == 0) return default(int);
            if (total == 0) return input;
            var result = $"{(decimal)input / total:#.##}";
            if (!string.IsNullOrEmpty(result))
            {
                return Convert.ToDouble(result);
            }

            return default(int);

        }

        public static int ToPercentageOf(this int input, int total, int division = 100)
        {
            if (input > total)
                return 100;

            if (total == 0)
                return 0;

            return (int)System.Math.Ceiling(decimal.Divide(input, total) * division);

        }

        public static string ToPositionInt(this int input)
        {

            var output = "th";
            var remainder = input % 100;

            if (remainder < 11 | remainder > 13)
            {
                switch (input % 10)
                {
                    case 1:
                        output = "st";
                        break;
                    case 2:
                        output = "nd";
                        break;
                    case 3:
                        output = "rd";
                        break;
                }

            }

            output = input + output;
            return output;

        }

        public static string ToPrettyTimeFromSeconds(this int? input)
        {
            if (input == null)
            {
                return default(int).ToString();
            }

            return input.Value.ToPrettyTimeFromSeconds();
        }

        public static string ToPrettyTimeFromSeconds(this int seconds)
        {

            var t = TimeSpan.FromSeconds(seconds);

            if (t.Days > 0)
            {
                return string.Format("{0:D1}d {1:D1}h",
                    t.Days,
                    t.Hours);
            }

            if (t.Hours > 0)
            {
                return string.Format("{0:D1}h {1:D1}m",
                    t.Hours,
                    t.Minutes);
            }

            if (t.Minutes > 0)
            {
                return string.Format("{0:D1}m {1:D1}s",
                    t.Minutes,
                    t.Seconds);
            }

            return string.Format("{0:D1}s", t.Seconds);

        }

    }

}
