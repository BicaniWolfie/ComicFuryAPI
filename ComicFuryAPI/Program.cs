using ComicFuryAPI;
using ComicFuryAPI.EndPoints;

APIApp app = new(args);

app.AddEndpoint(GetAuthorInfo.Instance);

app.Run();
