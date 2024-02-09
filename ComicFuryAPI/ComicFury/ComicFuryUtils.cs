using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ComicFuryAPI.ComicFury;

public static class ComicFuryUtils
{
    private static readonly ReadOnlyDictionary<string, byte> monthNum = new(
        new Dictionary<string, byte> {
            { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 },
            { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 }
        }
    );

    private static readonly Regex _regex = new(@"\d(?= days ago)*");
    public static DateOnly SanitizeDateString(string dateString)
    {
        
        if (dateString == "Today") return DateOnly.FromDateTime(DateTime.Now);
        if (dateString == "Yesterday") return DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        if (_regex.IsMatch(dateString))
        {
            byte.TryParse(_regex.Match(dateString).Value, out var days);
            return DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(days)));
        }
        
            
        var splitString = dateString.Split(' ');

        byte.TryParse(splitString[0].Substring(0, splitString[0].Length - 2), out var day) ;
        byte month = monthNum[splitString[1]];
        short.TryParse(splitString[2], out var year) ;
        
        return new DateOnly(year, month, day);
    }

    public static TimeOnly SanitizeTimeString(string timeString)
    {
        var splitString = timeString.Split(' ');
        var hourMinuteString = splitString[0].Split(':');

        byte.TryParse(hourMinuteString[0], out byte hour);
        byte.TryParse(hourMinuteString[1], out byte minute);
        hour += splitString[1] == "PM" ? (byte)12 : (byte)0;
        
        return new TimeOnly(hour, minute);
    }
}