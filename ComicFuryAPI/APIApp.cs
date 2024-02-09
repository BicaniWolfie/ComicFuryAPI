using ComicFuryAPI.EndPoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComicFuryAPI;

public class APIApp
{
    private WebApplication _app;

    public APIApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new(){Title = "Unofficial ComicFury API", Version = "v1"});
        });

        _app = builder.Build();
        
        
        // Configure the HTTP request pipeline.
        if (_app.Environment.IsDevelopment())
        {
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }

        _app.UseHttpsRedirection();
    }

    public void AddEndpoint(ComicEndpoint endpoint) => endpoint.AddEndpoint(_app);
    public void AddEndpoint(ComicEndpoint[] endpoints) {
        foreach(ComicEndpoint endpoint in endpoints)
        {
            endpoint.AddEndpoint(_app);
        }
    }

    public void Run() => _app.Run();
}