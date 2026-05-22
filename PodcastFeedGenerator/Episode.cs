using System;
using System.IO;

namespace PodcastFeedGenerator;

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
            
            return l / 1048576.0;
        }
    }
}