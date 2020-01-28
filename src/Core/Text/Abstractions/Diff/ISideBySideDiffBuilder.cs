using PlatoCore.Text.Abstractions.Diff.Models;

namespace PlatoCore.Text.Abstractions.Diff
{

    public interface ISideBySideDiffBuilder
    {
        SideBySideDiffModel BuildDiffModel(string oldText, string newText);
    }
}