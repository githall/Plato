using System.Text;
using System.Collections.Generic;
using Plato.Internal.Text.Abstractions;

namespace Plato.References.Services
{

    /// <summary>
    /// Creates tokens to identify the position of all hash references within text.
    /// A hash reference is identified with a hash (#) character followed by any number of
    /// digits representing the unique entity id until a terminating character or end of line is reached.
    /// Example text...
    /// Take a look at #1 and #12, or #123
    /// -----------------
    /// Where #1, #12, and #123 will be represented as tokens.
    /// </summary>
    public class HashTokenizer : IHashTokenizer
    {
        
        private const char StartChar = '#';

        // Denotes the end of a #reference
        private readonly IList<char> _terminators = new List<char>()
        {
            ',',
            '.',
            ' ',
            '\r',
            '\n',
            '\t',
            '<'
        };


        private readonly IList<char> _validChars = new List<char>()
        {
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '0'
        };

        public IEnumerable<Token> Tokenize(string input)
        {

            var start = 0;
            List<Token> output = null;
            StringBuilder sb = null;

            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];
                var startChar = c == StartChar;

                // Start char
                if (startChar)
                {
                    start = i;
                    sb = new StringBuilder();
                }

                if (sb != null)
                {
                    
                    var validChar = _validChars.Contains(c);
                    var isTerminator = _terminators.Contains(c);
                    var endOfInput = i == input.Length - 1;

                    // Not the start character or a terminator
                    if (!startChar && !isTerminator)
                    {
                        // Valid character?
                        if (validChar)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            // Token contains an invalid character - reset
                            start = 0;
                            sb = null;
                        }
                    }                   

                    // We've reached a terminator or the end of the input
                    if (isTerminator || endOfInput)
                    {
                        // Ensure the token has not been reest due to invalid characters
                        if (sb != null)
                        {
                            // Ensure we actually have a token value
                            if (!string.IsNullOrEmpty(sb.ToString()))
                            {
                                if (output == null)
                                {
                                    output = new List<Token>();
                                }
                                output.Add(new Token()
                                {
                                    Start = start,
                                    End = start + sb.ToString().Length,
                                    Value = sb.ToString()
                                });
                            }

                        }

                        start = 0;
                        sb = null;
                    }
                }

            }

            return output;

        }
        
    }
    
}
