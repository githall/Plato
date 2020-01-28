using PlatoCore.Text.Abstractions.Diff.Models;

namespace PlatoCore.Text.Abstractions.Diff
{
    public interface IInlineDiffBuilder
    {
        DiffPaneModel BuildDiffModel(string oldText, string newText);
    }
}
