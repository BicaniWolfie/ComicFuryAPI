using System.Text.Json;
using System.Threading.Tasks;
using ComicFuryAPI.ComicFury;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ComicFuryAPI.EndPoints;

internal record Params(string username);
public class GetProfileInfo: ComicEndpoint
{    
    private readonly string _baseUrl = "https://comicfury.com/profile.php";
    public override void AddEndpoint(WebApplication app)
    {
        app.MapGet("/profileinfo", Delegate)
            .WithName("GetProfileInfo")
            .WithOpenApi();
    }

    private async Task Delegate(HttpContext context)
    {
        var user = await JsonSerializer.DeserializeAsync<Params>(context.Request.Body);
        if (user is null)
        {
            context.Response.StatusCode = 404;
        }

        HtmlWeb web = new();
        HtmlDocument document = web.Load($"{_baseUrl}?username={user.username}");
        if (document.DocumentNode.SelectSingleNode("//*[@class='errorbox']") is not null)
        {
            context.Response.StatusCode = 404;
            return;
        }

        ComicFuryUtils.ProfileInfo profInfo = new();

        ComicFuryUtils.ReadInfo(ref profInfo, document);
        await context.Response.WriteAsJsonAsync(profInfo, JsonSerializerOptions.Default);
    }
}