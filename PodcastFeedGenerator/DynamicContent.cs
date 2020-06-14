using System.Xml;

namespace PodcastFeedGenerator
{
    public class DynamicContent
    {
        private readonly XmlDocument _dom;

        public DynamicContent()
        {
            _dom = new XmlDocument();
            _dom.Load("podcastsettings.xml");
        }

        private string GetValue(string element, bool asHtml = false)
        {
            var settingsElement = _dom.DocumentElement;
            
            var child = settingsElement?.SelectSingleNode(element);

            if (child == null)
                return "";

            return asHtml
                ? (child.InnerXml ?? "").Trim()
                : (child.InnerText ?? "").Trim();
        }
        
        public string PodcastFullTitle =>
            GetValue("PodcastFullTitle");

        public string SiteHeader =>
            GetValue("SiteHeader");

        public string RssCategory =>
            GetValue("RssCategory");
        
        public string PodcastTitleImage =>
            GetValue("PodcastTitleImage");

        public string TitleImageAlign =>
            GetValue("TitleImageAlign");

        public string TitleImageStyle =>
            GetValue("TitleImageStyle");

        public string RssDescription =>
            GetValue("RssDescription");
        
        public string Url =>
            GetValue("Url");

        public string Author =>
            GetValue("Author");

        public string AuthorMail =>
            GetValue("AuthorMail");

        public string ItunesUrl =>
            GetValue("ItunesUrl");

        public string SpotifyUrl =>
            GetValue("SpotifyUrl");
        
        public string DescriptionHtml =>
            GetValue("DescriptionHtml", true);
        
        public string QuestionsHtml =>
            GetValue("QuestionsHtml", true);

        public string BodyBackground =>
            GetValue("BodyBackground");

        public string BodyForeground =>
            GetValue("BodyForeground");

        public string LinkColor =>
            GetValue("LinkColor");

        public string LinkHover =>
            GetValue("LinkHover");

        public string ContentBackground =>
            GetValue("ContentBackground");

        public string ContentShadowColor =>
            GetValue("ContentShadowColor");

        public string ImageShadowColor =>
            GetValue("ImageShadowColor");
    }
}