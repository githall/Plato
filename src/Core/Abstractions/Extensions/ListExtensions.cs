using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatoCore.Abstractions.Extensions
{
    public static class ListExtensions
    {

        public static string ToDelimitedString(
            this IList<string> input,
            char delimiter = ',')
        {

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.ToArray().ToDelimitedString(delimiter);

        }

    }

}
