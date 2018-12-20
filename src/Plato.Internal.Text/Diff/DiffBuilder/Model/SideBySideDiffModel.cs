﻿namespace Plato.Internal.Text.Diff.DiffBuilder.Model
{
    /// <summary>
    /// A model which represents differences between two texts to be shown side by side
    /// </summary>
    public class SideBySideDiffModel
    {

        public DiffPaneModel OldText { get; }

        public DiffPaneModel NewText { get; }

        public SideBySideDiffModel()
        {
            OldText = new DiffPaneModel();
            NewText = new DiffPaneModel();
        }
    }
}