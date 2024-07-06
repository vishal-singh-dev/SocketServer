using Microsoft.Extensions.DependencyInjection;
using SocketServer.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var config = builder.Configuration;
builder.Services.AddSingleton<DataService>();
string file = Path.Combine(builder.Environment.ContentRootPath, config["FileName"]);
builder.Services.AddSingleton(new FolderService(file));
builder.Services.AddSingleton(new CryptoService(config["Secret"], config["Vector"]));
var app = builder.Build();
app.UseWebSockets();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
