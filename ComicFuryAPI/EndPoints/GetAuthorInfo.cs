using ComicFuryAPI.ComicFury;
using ComicFuryAPI.EndPoints.Utils;
using HtmlAgilityPack;

namespace ComicFuryAPI.EndPoints;

public class GetAuthorInfo: ComicEndpoint
{    
    private readonly string _baseUrl = "https://comicfury.com/profile.php";
    private HtmlDocument _document;
    public static readonly GetAuthorInfo Instance = new();
    public override void AddEndpoint(WebApplication app)
    {
        app.MapGet("/{user}/author", (string user) =>
            {
                _document = APIApp.webClient.Load($"{_baseUrl}?username={user}");
                if (_document.DocumentNode.SelectSingleNode("//*[@class='errorbox']") is not null)
                {
                    return Results.NotFound($"No user with username {user} exists.");
                }

                var profInfo = ComicFuryAuthorUtils.ReadInfo(_document);

                return Results.Ok(profInfo);
            })
            .WithName("GetAuthorInfo")
            .WithSummary("Gets author info for a given user.")
            .WithTags("info")
            .Produces<ComicFuryAuthorUtils.ProfileInfo>()
            .WithOpenApi();
    }
}