﻿using PlatoCore.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityReplyStore<TModel> : IStore<TModel> where TModel : class
    {
    }
    
}
