using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ComicFuryAPI.ComicFury;

public static class ComicFuryAuthorUtils
{
    public class ComicEntry
    {
        public string Title{get;set;}
        public string Desc{get;set;}
        public string Link{get;set;}
        public DateOnly LastUpdate{get;set;}
    }

    public class ProfileInfo
    {
        public string Username {get;set;}
        public DateOnly RegDate {get;set;}
        public DateTime LastSeen {get;set;}
        public short Posts {get;set;}
        public short Comments {get;set;}
        public short ProfileViews {get;set;}
        public List<ComicEntry> Comics {get;set;}
    }
    
    public static List<ComicEntry> GetComicEntries(HtmlDocument document)
    {
        List<ComicEntry> comicEntries = new();

        var nodes = document.DocumentNode
            .SelectSingleNode("//div[@class='usercomics']")
            .ChildNodes
            .Where(node => node.HasClass("usercomic"))
            .ToHashSet();
        foreach (var node in nodes)
        {
            ComicEntry entry = new();
            var decendants = node.Descendants().ToHashSet();
            Console.WriteLine();
            // var pcRight = node.SelectSingleNode("//div[@class='pcright']");
            entry.Title = decendants.First(childNode => childNode.HasClass("pcomictitle")).InnerHtml;
            entry.Desc = decendants.First(childNode => childNode.HasClass("pcright"))
                .Element("em")
                .InnerHtml
                .Replace("<br>", "\n");
            entry.Link = "https://comicfury.com" + decendants
                .First(childNode => childNode.HasClass("profile-webcomic-prof-link"))
                .Element("a")
                .GetAttributeValue("href", String.Empty);
            var dateInputString = decendants.
                First(childNode => childNode.HasClass("pcright"))
                .InnerText
                .Split("update:")[1]
                .Substring(0, 20)
                .Trim();
            entry.LastUpdate = ComicFuryUtils.SanitizeDateString(
                dateInputString
            );
            comicEntries.Add(entry);
        }

        return comicEntries;
    }


    public static ProfileInfo ReadInfo(HtmlDocument document)
    {
        ProfileInfo profInfo = new();
        var innerHtml = document.DocumentNode.SelectSingleNode("//*[@class='phead']").InnerHtml;
        profInfo.Username = innerHtml.Substring(0,innerHtml.Length - 10);
        
        var metaDataNode = document.DocumentNode.SelectSingleNode("//*[@class='user-metadata']");
        var info = metaDataNode
            .SelectNodes("//*[@class='authorinfo']/span[@class='info']")
            .Nodes()
            .Select(node => node.InnerHtml)
            .ToList();

        profInfo.RegDate = ComicFuryUtils.SanitizeDateString(info[0]);

        var dateTimeSplit = info[1].Split(", ");
        profInfo.LastSeen = ComicFuryUtils.SanitizeDateString(dateTimeSplit[0])
            .ToDateTime(ComicFuryUtils.SanitizeTimeString(dateTimeSplit[1]));

        short.TryParse(info[2], out var posts);
        profInfo.Posts = posts;

        short.TryParse(info[3], out var comments);
        profInfo.Comments = comments;

        short.TryParse(info[5], out var views);
        profInfo.ProfileViews = views;

        profInfo.Comics = GetComicEntries(document);

        return profInfo;
    }
}