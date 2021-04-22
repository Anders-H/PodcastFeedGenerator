using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PodcastFeedGenerator
{
    public class Episode
    {
        public int EpisodeNumber { get; }
        public string Title { get; }
        public string Mp3Filename { get; }
        public DateTime PublishedDate { get; }
        public string Length { get; }
        public string Description { get; }
        public string TitleImage { get; }
        public string HtmlPodcastDescription { get; }
        public string HtmlPodcastQuestions { get; }
        
        public Episode(
            int episodeNumber,
            string title,
            string filename,
            DateTime publishedDate,
            string length,
            string description,
            string titleImage,
            string htmlPodcastDescription,
            string htmlPodcastQuestions)
        {
            EpisodeNumber = episodeNumber;
            Title = title;
            Mp3Filename = $"{filename}.mp3";
            PublishedDate = publishedDate;
            Length = length;
            Description = description;
            TitleImage = titleImage;
            HtmlPodcastDescription = htmlPodcastDescription;
            HtmlPodcastQuestions = htmlPodcastQuestions;
        }
        
        public long LengthBytes
        {
            get
            {
                var fi = new FileInfo($@"output\mp3\{Mp3Filename}");
                return fi.Exists ? fi.Length : 0L;
            }
        }
        
        public double LengthMb
        {
            get
            {
                var l = LengthBytes;
                if (l <= 0)
                    return 0.0;
                return l/1048576.0;
            }
        }
    }
    
    public class EpisodeList : List<Episode>
    {
        public void Load(string filename)
        {
            Clear();
            string xml;
            using (var sr = new StreamReader(filename, Encoding.UTF8))
            {
                xml = sr.ReadToEnd();
                sr.Close();
            }
            var dom = new XmlDocument();
            dom.LoadXml(xml);
            var episodes = dom.DocumentElement?.SelectSingleNode("episodes");
            if (episodes == null)
                throw new Exception("No episodes!");
            var episodeList = episodes.SelectNodes("episode");
            if (episodeList == null || episodeList.Count <= 0)
                return;
            var epNum = episodeList.Count;
            
            var dynamicContent = new DynamicContent();
            
            foreach (XmlElement episode in episodes)
            {
                var title = episode.SelectSingleNode("title")?.InnerText ?? "";
                var mp3Filename = episode.SelectSingleNode("filename")?.InnerText ?? "";
                var publishdate = episode.SelectSingleNode("publishdate")?.InnerText ?? "";
                var length = episode.SelectSingleNode("length")?.InnerText ?? "";
                var description = episode.SelectSingleNode("description")?.InnerText ?? "";
                var titleImage = dynamicContent.PodcastTitleImage;
                var htmlPodcastDescription = episode.SelectSingleNode("htmlPodcastDescription")?.InnerXml ?? "";
                var htmlPodcastQuestions = episode.SelectSingleNode("htmlPodcastQuestions")?.InnerXml ?? "";
                
                var x = new Episode(
                    epNum,
                    title, mp3Filename, publishdate.AsDate(),
                    length, description,
                    titleImage,
                    htmlPodcastDescription,
                    htmlPodcastQuestions
                );
                
                Add(x);
                epNum--;
            }
        }
    }
}