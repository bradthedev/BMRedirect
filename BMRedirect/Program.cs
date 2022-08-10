using BMRedirect.Api.Configurations;
using BMRedirect.Core;
using BMRedirect.Core.Middleware;
using BMRedirect.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRedirectService, RedirectService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseAuthorization();

app.MapControllers();

app.UseProxyMiddlware();

app.Run();

