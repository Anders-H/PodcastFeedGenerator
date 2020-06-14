using System;

namespace PodcastFeedGenerator
{
    public static class Extensions
    {
        public static DateTime AsDate(this string me)
            => DateTime.Parse(me);
    }
}
