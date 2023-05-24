namespace Bu.Web.Data.Contexts;

public abstract class CommonCachedContext<TContext, TCache> : ICommonCachedContext
    where TContext : ICommonContext
    where TCache : CommonCachedContext<TContext, TCache>.CommonCache, new()
{
    protected Func<TCache> GetCache { get; }

    protected CommonCachedContext(Func<TContext> contextProvider, IMemoryCache memoryCache, TimeSpan cacheExpiryTimeSpan)
    {
        this.GetCache = () =>
        {
            return memoryCache.GetOrCreate(nameof(AdminCachedContext), entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheExpiryTimeSpan;
                using TContext context = contextProvider();
                TCache cache = new();
                cache.Init(context);
                return cache;
            }) ?? throw new InvalidOperationException();
        };
    }

    public abstract class CommonCache : ICommonCachedContext
    {
        public abstract void Init(TContext context);
    }
}