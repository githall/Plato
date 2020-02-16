namespace PlatoCore.Stores.Abstractions
{

    public interface ICacheableStore<TModel> where TModel : class
    {
        void CancelTokens(TModel model);
    }

}
