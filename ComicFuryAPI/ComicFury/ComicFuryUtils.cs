using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HtmlAgilityPack;

namespace ComicFuryAPI.ComicFury;

public static class ComicFuryUtils
{
    private static readonly ReadOnlyDictionary<string, byte> monthNum = new(
        new Dictionary<string, byte> {
            { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 },
            { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 }
        }
    );
    public class ComicEntry
    {
        public string Title;
        public string Desc;
        public string Link;
        public DateOnly LastUpdate;
    }

    public class ProfileInfo
    {
        public string Username;
        public DateOnly RegDate;
        public DateTime LastSeen;
        public short Posts;
        public short Comments;
        public short ProfileViews;
        public ComicEntry[] Comics;
    }
    
    public static ComicEntry[] GetComicEntries(HtmlDocument document)
    {
        var comicNodes = document.DocumentNode.SelectSingleNode("//*[@class='usercomics']")
            .Elements("div");
        return comicNodes.Select(node =>
        {
            ComicFuryUtils.ComicEntry entry = new ComicFuryUtils.ComicEntry();
            entry.Title = node.SelectSingleNode("//a[@class='pcomictitle']").InnerHtml;
            entry.Desc = node.SelectSingleNode("//div[@class='pcright']/em").InnerHtml.Replace("<br>", "\n");
            entry.Link = "https://comicfury.com/" + node
                .SelectSingleNode("//div[@class='profile-webcomic-prof-link']/a[@href]")
                .GetAttributeValue("href", String.Empty);

            var test = node.SelectSingleNode("//div[@class='pcright']").InnerText;
            entry.LastUpdate = SanitizeDateString(test.Split("update:")[1].Substring(0,16).Trim());
            return entry;
        }).ToArray();
    }
    
    
    public static void ReadInfo(ref ProfileInfo profInfo, HtmlDocument document)
    {
        var innerHtml = document.DocumentNode.SelectSingleNode("//*[@class='phead']").InnerHtml;
        profInfo.Username = innerHtml.Substring(0,innerHtml.Length - 10);
        
        var metaDataNode = document.DocumentNode.SelectSingleNode("//*[@class='user-metadata']");
        var info = metaDataNode.SelectNodes("//*[@class='authorinfo']/span[@class='info']")
            .Nodes()
            .Select(node => node.InnerHtml)
            .ToArray();

        profInfo.RegDate = SanitizeDateString(info[0]);

        var dateTimeSplit = info[1].Split(", ");
        profInfo.LastSeen = SanitizeDateString(dateTimeSplit[0]).ToDateTime(SanitizeTimeString(dateTimeSplit[1]));

        short.TryParse(info[2], out var posts);
        profInfo.Posts = posts;

        short.TryParse(info[3], out var comments);
        profInfo.Comments = comments;

        short.TryParse(info[5], out var views);
        profInfo.ProfileViews = views;

        profInfo.Comics = GetComicEntries(document);
    }

    private static DateOnly SanitizeDateString(string dateString)
    {
        if (dateString == "Today") return DateOnly.FromDateTime(DateTime.Now);
        if (dateString == "Yesterday") return DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        
        var splitString = dateString.Split(' ');

        byte.TryParse(splitString[0].Substring(0, splitString[0].Length - 2), out var day) ;
        byte month = monthNum[splitString[1]];
        short.TryParse(splitString[2], out var year) ;
        
        return new DateOnly(year, month, day);
    }

    private static TimeOnly SanitizeTimeString(string timeString)
    {
        var splitString = timeString.Split(' ');
        var hourMinuteString = splitString[0].Split(':');

        byte.TryParse(hourMinuteString[0], out byte hour);
        byte.TryParse(hourMinuteString[1], out byte minute);
        hour += splitString[1] == "PM" ? (byte)12 : (byte)0;
        
        return new TimeOnly(hour, minute);
    }
}