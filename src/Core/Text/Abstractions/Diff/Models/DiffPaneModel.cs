using System.Collections.Generic;

namespace PlatoCore.Text.Abstractions.Diff.Models
{
    public class DiffPaneModel
    {
        public List<DiffPiece> Lines { get; }

        public DiffPaneModel()
        {
            Lines = new List<DiffPiece>();
        }
    }
}