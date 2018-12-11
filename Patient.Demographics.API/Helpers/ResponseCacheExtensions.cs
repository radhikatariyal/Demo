using System;
using System.Web;
using System.Web.Mvc;

namespace Patient.Demographics.API.Helpers
{
    public static class ResponseCacheExtensions
    {
        public static void DisableBrowserCachingSoLoggedOutUsersCantSeeCachedPages(this HttpCachePolicy cache)
        {
            // Adapted from http://stackoverflow.com/questions/10011780/prevent-caching-in-asp-net-mvc-for-specific-actions-using-an-attribute
            cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            cache.SetValidUntilExpires(false);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetNoStore();
        }
    }
}