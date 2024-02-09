using Microsoft.AspNetCore.Builder;

namespace ComicFuryAPI.EndPoints;

public abstract class ComicEndpoint
{
    public abstract void AddEndpoint(WebApplication app);
}