using API.Infrastructure.Data;
using API.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    app.SeedData();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("ERRO AO INICIAR A APLICAÇÃO");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

    var url = "https://webhook.site/ca882dca-ab0f-49fa-83d1-a6794909fbfa";
    var client = new HttpClient();
    var json = new
    {
        text = $"{ex.Message} \n {ex.StackTrace}",
    };

    var output = JsonSerializer.Serialize(json);
    await client.PostAsync(url, new StringContent(output, Encoding.UTF8, "application/json"));
    throw;
}
