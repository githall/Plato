using System;
using System.Collections.Generic;
using System.Text;

namespace PlatoCore.Abstractions.Extensions
{

    public static class ArrayExtensions
    {
        public static IList<string> ToList(this string[] input)
        {

            var output = new List<string>(input.Length - 1);
            if (input != null)
            {
                for (var i = 0; i <= input.Length - 1; i++)
                {
                    output.Add(input[i]);
                }
            }

            return output;

        }

        public static string ToDelimitedString(
            this string[] input,
            char delimiter = ',')
        {

            var sb = new StringBuilder();
            if (input != null)
            {
                for (var i = 0; i <= input.Length - 1; i++)
                {
                    sb.Append(input.GetValue(i).ToString())
                        .Append(delimiter);
                }
            }

            return sb.ToString().TrimEnd(delimiter);

        }

        public static string ToDelimitedString(
            this int[] input, 
            char delimiter = ',')
        {

            var sb = new StringBuilder();
            if (input != null)
            {
                for (var i = 0; i <= input.Length - 1; i++)
                {
                    sb.Append(input.GetValue(i).ToString())
                        .Append(delimiter);
                }
            }
            return sb.ToString().TrimEnd(delimiter);

        }

        public static bool Contains(this string[] inputs, string value)
        {
            return inputs.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(
            this string[] inputs,
            string value,
            StringComparison comparer)
        {

            if (inputs == null)
            {
                return false;
            }

            foreach (string input in inputs)
            {
                if (input.Equals(value, comparer))
                {
                    return true;
                }
            }

            return false;

        }

        public static bool Contains(
            this int[] inputs,
            int value)
        {

            if (inputs == null)
            {
                return false;
            }

            foreach (int input in inputs)
            {
                if (input == value)
                {
                    return true;
                }
            }

            return false;

        }

    }

}
