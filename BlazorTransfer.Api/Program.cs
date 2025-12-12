using System.Text.Json;
using BlazorTransfer.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<FileStorageService>();
builder.Services.AddHostedService<FileCleanupWorker>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:6501") // Client URL
              .AllowAnyHeader()
              .AllowAnyMethod());
});



var app = builder.Build();


app.UseCors();
app.MapControllers();
app.Run();