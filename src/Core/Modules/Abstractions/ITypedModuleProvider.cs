using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using PlatoCore.Models.Modules;

namespace PlatoCore.Modules.Abstractions
{
    public interface ITypedModuleProvider
    {
        Task<IModuleEntry> GetModuleForDependency(Type dependency);

        Task<IDictionary<Type, IModuleEntry>> GetModuleDependenciesAsync(IEnumerable<IModuleEntry> modules);

        Task<Type> GetTypeCandidateAsync(string typeName, Type baseType);
        
        Task<IEnumerable<TypeInfo>> GetTypesAsync();


    }
}
