using System;
using System.IO;

namespace PlatoCore.Drawing.Abstractions.Letters
{
    public interface ILetterRenderer : IDisposable
    {
        Stream GetLetter(LetterOptions options);
    }

}
