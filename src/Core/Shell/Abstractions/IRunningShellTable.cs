using System.Collections.Generic;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{
    public interface IRunningShellTable
    {

        void Add(IShellSettings settings);

        void Remove(IShellSettings settings);

        IShellSettings Match(string host, string appRelativeCurrentExecutionFilePath);

        IDictionary<string, IShellSettings> ShellsByHostAndPrefix { get; }

    }

}
