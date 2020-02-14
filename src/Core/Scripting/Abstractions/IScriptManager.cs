namespace PlatoCore.Scripting.Abstractions
{
    public interface IScriptManager
    {
        ScriptBlocks GetScriptBlocks(ScriptSection section);

        void RegisterScriptBlock(ScriptBlock block, ScriptSection section);

    }

}
