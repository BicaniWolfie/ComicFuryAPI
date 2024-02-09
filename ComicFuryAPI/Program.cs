using ComicFuryAPI;
using ComicFuryAPI.EndPoints;

APIApp app = new(args);

app.AddEndpoint([
    new WeatherForecast(),
    new GetProfileInfo()
]);

app.Run();
