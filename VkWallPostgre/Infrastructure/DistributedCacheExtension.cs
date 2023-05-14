using Microsoft.Extensions.Caching.Distributed;
using VkWallPostgre.Dto;

namespace VkWallPostgre.Infostracture
{
    public static class DistributedCacheExtension
    {
        private const string VkTokenKey = "VkToken";
        public static string? GetVkToken(this IDistributedCache cache)
        {
            return cache.GetString(VkTokenKey);
        }

        public static void SetVkToken(this IDistributedCache cache, VkToken token)
        {
            cache.SetString(VkTokenKey, token.Token, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.Expired - 60)
            });
        }
    }
}
