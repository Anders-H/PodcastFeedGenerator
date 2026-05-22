using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PodcastFeedGenerator;

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

        if (episodeList is not { Count: > 0 })
            return;

        var epNum = episodeList.Count;
        var dynamicContent = new DynamicContent();

        foreach (XmlElement episode in episodes)
        {
            var title = episode.SelectSingleNode("title")?.InnerText ?? "";
            var mp3Filename = episode.SelectSingleNode("filename")?.InnerText ?? "";
            var publishDate = episode.SelectSingleNode("publishDate")?.InnerText ?? "";
            var length = episode.SelectSingleNode("length")?.InnerText ?? "";
            var description = episode.SelectSingleNode("description")?.InnerText ?? "";
            var titleImage = dynamicContent.PodcastTitleImage;
            var htmlPodcastDescription = episode.SelectSingleNode("htmlPodcastDescription")?.InnerXml ?? "";
            var htmlPodcastQuestions = episode.SelectSingleNode("htmlPodcastQuestions")?.InnerXml ?? "";

            var x = new Episode(
                epNum,
                title, mp3Filename, publishDate.AsDate(),
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