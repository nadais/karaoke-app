using Karaoke_catalog_server;
using Karaoke.Api;
using Karaoke.Api.Settings;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>  
{  
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Karaoke admin", Version = "v1" });  
    c.OperationFilter<SwaggerFileOperationFilter>();  
});
var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL");
if(redisUrl != null)
{ 
    builder.Services.AddStackExchangeRedisCache(options => 
    { 
        var tokens = redisUrl.Split(':', '@');
        options.ConfigurationOptions = ConfigurationOptions.Parse($"{tokens[3]}:{tokens[4]},password={tokens[2]}"); 
    });
}
builder.Services.Configure<SongsCollectionSettings>(
    builder.Configuration.GetSection("SongsDatabase"));
builder.Services.AddTransient(typeof(MongoDbService));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
var mongoDbService = app.Services.GetRequiredService<MongoDbService>();
await mongoDbService.AddSongsCollectionIfNotExistsAsync();

app.UseCors(options => options.AllowAnyOrigin());

app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();


app.Run();