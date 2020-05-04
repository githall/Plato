namespace PlatoCore.Abstractions.Extensions
{

    public static class CharExtensions
    {

        public static string Repeat(this char c, int times)
        {
            return c.ToString().Repeat(times);
        }

        public static bool IsValidBase64Char(this char c)
        {
            var intValue = (int)c;
            if (intValue >= 48 && intValue <= 57)
            {
                return false;
            }

            if (intValue >= 65 && intValue <= 90)
            {
                return false;
            }

            if (intValue >= 97 && intValue <= 122)
            {
                return false;
            }
            return intValue != 43 && intValue != 47;
        }

        public static bool IsDigit(this char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsLower(this char c)
        {
            return c >= 'a' && c <= 'z';
        }

        public static bool IsUpper(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        public static bool IsSpace(this char c)
        {
            return c == ' ';
        }

        public static bool IsLetterOrDigit(this char c)
        {
            return c.IsUpper() || c.IsLower() || c.IsDigit();
        }

        public static bool IsLetterDigitOrSpace(this char c)
        {
            return c.IsLetterOrDigit() || c.IsSpace();
        }

    }

}
