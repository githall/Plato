using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Extensions;

namespace PlatoCore.Cache
{

    //public interface IScopedCacheManager : ICacheManager
    //{

    //}

    //public class ScopedCacheManager : IScopedCacheManager
    //{

    //    public static ConcurrentDictionary<CacheToken, Type> Tokens { get; } =
    //        new ConcurrentDictionary<CacheToken, Type>();

    //    public static ConcurrentDictionary<string, CacheToken> Caches { get; } =
    //      new ConcurrentDictionary<string, CacheToken>();

    //    private readonly IShellSettings _shellSettings;
    //    private readonly ICacheManager _cacheManager;

    //    public ScopedCacheManager(
    //        IShellSettings shellSettings,
    //        ICacheManager cacheManager)
    //    {
    //        _shellSettings = shellSettings;
    //        _cacheManager = cacheManager;
    //    }

    //    public CacheToken GetOrCreateToken(Type type, params object[] varyBy)
    //    {
    //        var cacheToken = new CacheToken(_shellSettings.Name, type, varyBy);
    //        if (Tokens.ContainsKey(cacheToken))
    //        {
    //            return Tokens.FirstOrDefault(t => t.Key == cacheToken).Key;
    //        }

    //        Tokens.TryAdd(cacheToken, type);
    //        return cacheToken;
    //    }          

    //    public Task<TItem> GetOrCreateAsync<TItem>(CacheToken token, Func<ICacheEntry, Task<TItem>> factory)
    //        => _cacheManager.GetOrCreateAsync<TItem>(token, factory);

    //    public void CancelToken(CacheToken token)
    //    {

    //        if (Tokens.ContainsKey(token))
    //        {
    //            Tokens.TryRemove(token, out Type type);
    //        }

    //         _cacheManager.CancelToken(token);

    //    }

    //    public void CancelTokens(Type type)
    //    {
    //        var tokens = GetTokensForType(type);
    //        foreach (var token in tokens)
    //        {
    //            CancelToken(token);
    //        }
    //    }

    //    public void CancelTokens(Type type, params object[] varyBy)
    //    {
    //        var cancellationToken = new CacheToken(_shellSettings.Name, type, varyBy);
    //        var tokens = GetTokensForType(type);
    //        foreach (var token in tokens)
    //        {
    //            if (cancellationToken == token)
    //            {
    //                CancelToken(token);
    //            }

    //        }
    //    }

    //    // ---------------------------

    //    IEnumerable<CacheToken> GetTokensForType(Type type)
    //    {
    //        return Tokens
    //            .Where(t => t.Value == type)
    //            .Select(c => c.Key)
    //            .ToList();
    //    }

    //}

}
