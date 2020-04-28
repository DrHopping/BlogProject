using System;
using System.Collections.Generic;

namespace BLL.Helpers
{
    public static class DefaultAvatarUrlProvider
    {
        private static readonly Random Rand = new Random();
        private static readonly List<string> Urls = new List<string>
        {
            "https://ramcotubular.com/wp-content/uploads/default-avatar.jpg"
        };
        public static string GetDefaultAvatarUrl()
        {
            return Urls[Rand.Next(0, Urls.Count - 1)];
        }
    }
}