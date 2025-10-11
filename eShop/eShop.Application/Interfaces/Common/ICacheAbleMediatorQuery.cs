
namespace eShop.Application.Interfaces
{
    public interface ICacheAbleMediatorQuery
    {
        bool BypassCache { get; }
        string CacheKey { get; }
        TimeSpan? SlidingExpiration { get; }
    }
}
