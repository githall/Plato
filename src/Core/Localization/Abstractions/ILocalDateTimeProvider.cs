using System;
using System.Threading.Tasks;

namespace PlatoCore.Localization.Abstractions
{

    public interface ILocalDateTimeProvider
    {
        Task<DateTimeOffset> GetLocalDateTimeAsync(LocalDateTimeOptions options);

    }
    
}
