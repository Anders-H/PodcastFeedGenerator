using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PodcastFeedGenerator
{
    public class Program
    {
        private const int MaxItemsRss = 1000000;
        private static readonly DynamicContent ContentData = new DynamicContent();
        private static EpisodeList Episodes { get; } = new EpisodeList();

        private static void Main()
        {
            Episodes.Load(@"source.xml");
            CreateFeed();
            CreateWebSite();
        }

        private static void CreateFeed()
        {
            #region XmlTemplate
            var head = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0""
    xmlns:content=""http://purl.org/rss/1.0/modules/content/""
    xmlns:wfw=""http://wellformedweb.org/CommentAPI/""
    xmlns:dc=""http://purl.org/dc/elements/1.1/""
    xmlns:atom=""http://www.w3.org/2005/Atom""
    xmlns:sy=""http://purl.org/rss/1.0/modules/syndication/""
    xmlns:slash=""http://purl.org/rss/1.0/modules/slash/""
    xmlns:itunes=""http://www.itunes.com/dtds/podcast-1.0.dtd""
xmlns:rawvoice=""http://www.rawvoice.com/rawvoiceRssModule/""
xmlns:googleplay=""http://www.google.com/schemas/play-podcasts/1.0"">
  <channel>
    <title>{ContentData.PodcastFullTitle}</title>
    <category>{ContentData.RssCategory}</category>
    <atom:link href=""{ContentData.Url}rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <link>{ContentData.Url}</link>
    <description>{ContentData.RssDescription}</description>
    <lastBuildDate>@BuildDate@</lastBuildDate>
    <language>sv-SE</language>
    <sy:updatePeriod>weekly</sy:updatePeriod>
    <sy:updateFrequency>1</sy:updateFrequency>
    <generator>Custom</generator>
    <itunes:summary>
      {ContentData.RssDescription}
    </itunes:summary>
    <itunes:author>{ContentData.Author}</itunes:author>
    <itunes:explicit>No</itunes:explicit>
    <itunes:image href=""{ContentData.Url}{ContentData.PodcastTitleImage}"" />
    <itunes:owner>
      <itunes:name>{ContentData.Author}</itunes:name>
      <itunes:email>{ContentData.AuthorMail}</itunes:email>
    </itunes:owner>
    <managingEditor>{ContentData.AuthorMail}</managingEditor>
    <copyright>{ContentData.Author}</copyright>
    <itunes:subtitle>{ContentData.PodcastFullTitle}</itunes:subtitle>
    <image>
      <title>{ContentData.PodcastFullTitle}</title>
      <url>{ContentData.Url}{ContentData.PodcastTitleImage}</url>
      <link>{ContentData.Url}</link>
    </image>
    <itunes:category text=""{ContentData.RssCategory}"">
    </itunes:category>
    <googleplay:email>{ContentData.AuthorMail}</googleplay:email>
    <googleplay:description>{ContentData.RssDescription}</googleplay:description>
    <googleplay:explicit>No</googleplay:explicit>
    <googleplay:category text=""{ContentData.RssCategory}"" />
    <googleplay:image href=""{ContentData.Url}{ContentData.PodcastTitleImage}"" />
    <rawvoice:rating>TV-G</rawvoice:rating>
    <rawvoice:frequency>Weekly</rawvoice:frequency>
    <rawvoice:subscribe feed=""{ContentData.Url}rss.xml"" googleplay=""{ContentData.Url}rss.xml""></rawvoice:subscribe>";

            string item = $@"<item>
        <title>@Title@</title>
        <link>@guid@</link>
        <pubDate>@PubDate@</pubDate>
        <guid isPermaLink=""false"">@guid@</guid>
        <description>@Description@</description>
        <content:encoded><![CDATA[@HtmlDescription@
]]></content:encoded>
        <enclosure url=""{ContentData.Url}mp3/@Mp3File@"" length=""@Mp3Length@"" type=""audio/mpeg"" />
        <itunes:subtitle>@ShortDescription@</itunes:subtitle>
        <itunes:summary>@ShortDescription@</itunes:summary>
        <itunes:author>{ContentData.Author}</itunes:author>
        <itunes:image href=""{ContentData.Url}{ContentData.PodcastTitleImage}"" />
        <itunes:explicit>No</itunes:explicit>
        <itunes:duration>@Duration@</itunes:duration>
</item>
";
            const string foot = @"</channel>
</rss>
";
            #endregion

            var fileInfo = new FileInfo(@"output/rss.xml");

            var directory = fileInfo.Directory;

            if (directory == null)
                throw new SystemException("Directory is null.");

            if (!directory.Exists)
                directory.Create();

            using (var sw = new StreamWriter(@"output/rss.xml", false, Encoding.UTF8))
            {
                sw.WriteLine(head.Replace("@BuildDate@", DateString(DateTime.Now)));
                var episodeCount = 0;
                foreach (var e in Episodes)
                {
                    episodeCount++;
                    var i = item.Replace("@Title@", $@"{e.Title}");
                    i = i.Replace("@Mp3Length@", e.LengthBytes.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    i = i.Replace("@PubDate@", DateString(e.PublishedDate));
                    i = i.Replace("@Description@", e.Description);
                    i = i.Replace("@HtmlDescription@", $"<p>{e.Description}</p>");
                    i = i.Replace("@Mp3File@", e.Mp3Filename);
                    i = i.Replace("@ShortDescription@", e.Description);
                    i = i.Replace("@Duration@", $"{e.Length}");
                    i = i.Replace("@guid@", $"{ContentData.Url}{e.EpisodeNumber:00}.html");
                    sw.WriteLine(i);
                    if (episodeCount >= MaxItemsRss)
                        break;
                }
                sw.WriteLine(foot);
            }
        }
        private static void CreateWebSite()
        {
            #region XmlTemplate
            var template = $@"
<!DOCTYPE html>
<html lang=""sv"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
<meta charset=""utf-8"" />
<link rel=""apple-touch-icon"" sizes=""57x57"" href=""/apple-icon-57x57.png"">
<link rel=""apple-touch-icon"" sizes=""60x60"" href=""/apple-icon-60x60.png"">
<link rel=""apple-touch-icon"" sizes=""72x72"" href=""/apple-icon-72x72.png"">
<link rel=""apple-touch-icon"" sizes=""76x76"" href=""/apple-icon-76x76.png"">
<link rel=""apple-touch-icon"" sizes=""114x114"" href=""/apple-icon-114x114.png"">
<link rel=""apple-touch-icon"" sizes=""120x120"" href=""/apple-icon-120x120.png"">
<link rel=""apple-touch-icon"" sizes=""144x144"" href=""/apple-icon-144x144.png"">
<link rel=""apple-touch-icon"" sizes=""152x152"" href=""/apple-icon-152x152.png"">
<link rel=""apple-touch-icon"" sizes=""180x180"" href=""/apple-icon-180x180.png"">
<link rel=""icon"" type=""image/png"" sizes=""192x192""  href=""/android-icon-192x192.png"">
<link rel=""icon"" type=""image/png"" sizes=""32x32"" href=""/favicon-32x32.png"">
<link rel=""icon"" type=""image/png"" sizes=""96x96"" href=""/favicon-96x96.png"">
<link rel=""icon"" type=""image/png"" sizes=""16x16"" href=""/favicon-16x16.png"">
<link rel=""manifest"" href=""/manifest.json"">
<meta name=""msapplication-TileColor"" content=""#ffffff"">
<meta name=""msapplication-TileImage"" content=""/ms-icon-144x144.png"">
<meta name=""theme-color"" content=""#ffffff"">
<title>{ContentData.PodcastFullTitle}</title>
<style type=""text/css"">
    body {{
        margin: 10px 0 10px 0;
        padding: 0;
        font-family: arial;
        font-size: 15px;
        font-weight: normal;
        background-color: {ContentData.BodyBackground};
        color: {ContentData.BodyForeground};
    }}

    a {{
        color: {ContentData.LinkColor};
        text-decoration: none;
    }}

    a:hover {{
        color: {ContentData.LinkHover};
    }}

    div {{
        margin: 0 auto;
        width: 90%;
        max-width: 700px;
        background-color: {ContentData.ContentBackground};
        padding: 2px 12px 2px 12px;
        box-shadow: 1px 1px 9px {ContentData.ContentShadowColor};
    }}

    p {{
        margin: 8px 0 16px 0;
        padding: 0;
    }}

    h1 {{
        font-family: arial;
        font-size: 24px;
        font-weight: bold;
        margin: 8px 0 16px 0;
        padding: 0;
    }}

    h2 {{
        font-family: arial;
        font-size: 18px;
        font-weight: bold;
        font-style: italic;
        margin: 8px 0 16px 0;
        padding: 0;
    }}

    img {{
        box-shadow: 1px 1px 9px {ContentData.ImageShadowColor};
    }}
</style>
</head>
<body>
<div>
@Content@
</div>
</body>
</html>
";
            #endregion
            var s = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ContentData.SiteHeader))
                s.AppendLine($"<h1>{ContentData.SiteHeader}</h1>");
            s.Append($"<p style=\"text-align: {ContentData.TitleImageAlign};\"><img src=\"{ContentData.Url}{ContentData.PodcastTitleImage}\" style=\"{ContentData.TitleImageStyle}\" alt=\"{ContentData.RssDescription}\"/></p>");
            s.Append(ContentData.DescriptionHtml);
            s.Append(ContentData.QuestionsHtml);
            var url = $"{ContentData.Url}mp3/{Episodes.First().Mp3Filename}";
            s.Append($@"<audio style=""width: 100%;"" controls>
<source src=""{url}"" type=""audio/mpeg"">
</audio>");
            s.Append($@"<p><b>Ladda hem det senaste avsnittet:</b> <a href=""{url}"" target=""_blank"">{Episodes.First().Mp3Filename}</a> ({Episodes.First().Length} minuter, {Episodes.First().LengthMb:n1} Mb)</p>");
            s.Append("<p>");

            s.Append("<p></p>");
            AppendPrenumerera(ref s);

            s.Append(@"<h1>Alla avsnitt</h1>");

            foreach (var episode in Episodes)
            {
                s.AppendLine();
                s.AppendLine();
                s.Append($@"<h2>Avsnitt {episode.EpisodeNumber}: <a href=""{ContentData.Url}{episode.EpisodeNumber:00}.html"">{episode.Title}</a></h2>");
                s.Append($"<p><b>Längd:</b> {episode.Length}</p>");
                s.Append($"<p><b>Filstorlek:</b> {episode.LengthMb:n1} Mb</p>");
                var lurl = $"{ContentData.Url}mp3/{episode.Mp3Filename}";
                s.Append($@"<p><b>Ladda hem:</b> <a href=""{lurl}"" target=""_blank"">{lurl}</a></p>");
                s.Append($"<p>{episode.Description}</p>");
                s.Append("<p></p>");
                s.AppendLine();
                s.AppendLine();
            }

            using (var sw = new StreamWriter(@"output/index.html", false, Encoding.UTF8))
            {
                var website = template.Replace("@Content@", s.ToString());
                sw.Write(website);
                sw.Flush();
                sw.Close();
            }

            foreach (var episode in Episodes)
            {
                s = new StringBuilder();

                s.AppendLine(!string.IsNullOrWhiteSpace(ContentData.SiteHeader)
                    ? $"<h1>{ContentData.PodcastFullTitle} {episode.Title}</h1>"
                    : $"<h1>{episode.Title}</h1>");

                s.Append($"<p style=\"text-align: {ContentData.TitleImageAlign};\"><img src=\"{ContentData.Url}{episode.TitleImage}\" style=\"{ContentData.TitleImageStyle}\" alt=\"{episode.Title}\"/></p>");
                s.Append(episode.HtmlPodcastDescription);
                s.Append($"<p>{episode.Description}</p>");
                url = $"{ContentData.Url}mp3/{episode.Mp3Filename}";
                s.Append($@"<audio style=""width: 100%;"" controls>
<source src=""{url}"" type=""audio/mpeg"">
</audio>");
                s.Append("<p>");

                s.Append("<p></p>");
                AppendPrenumerera(ref s);

                s.Append($"<p><b>Längd:</b> {episode.Length}</p>");
                s.Append($"<p><b>Filstorlek:</b> {episode.LengthMb:n1} Mb</p>");
                var lurl = $"{ContentData.Url}mp3/{episode.Mp3Filename}";
                s.Append($@"<p><b>Ladda hem:</b> <a href=""{lurl}"" target=""_blank"">{lurl}</a></p>");
                s.Append("<p></p>");
                s.Append($@"<p><a href=""{ContentData.Url}"">Tillbaka</a></p>");
                s.Append("<p></p>");
                using (var sw = new StreamWriter($@"output/{episode.EpisodeNumber:00}.html", false, Encoding.UTF8))
                {
                    var website = template.Replace("@Content@", s.ToString());
                    sw.Write(website);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        private static void AppendPrenumerera(ref StringBuilder s)
        {
            s.Append("<p>Prenumerera: ");
            s.Append($@"<a href=""{ContentData.Url}rss.xml"" target=""_blank"">RSS</a>");

            if (!string.IsNullOrWhiteSpace(ContentData.ItunesUrl))
                s.Append($@"/<a href=""{ContentData.ItunesUrl}"" target=""_blank"">iTunes</a>");
            
            if (!string.IsNullOrWhiteSpace(ContentData.SpotifyUrl))
                s.Append($@"/<a href=""{ContentData.SpotifyUrl}"" target=""_blank"">Spotify</a>");

            s.Append("</p>");
        }

        private static string DateString(DateTime pubDate) =>
            GetRfc822Date(pubDate);
        
        private static string GetRfc822Date(DateTime date)
        {
            var offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            var timeZone = "+" + offset.ToString().PadLeft(2, '0');
            if (offset < 0)
            {
                // ReSharper disable once IntVariableOverflowInUncheckedContext
                var i = offset * -1;
                timeZone = "-" + i.ToString().PadLeft(2, '0');
            }
            return date.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone.PadRight(5, '0'), System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}