using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pudra.Connect.Api.Extensions;
using Pudra.Connect.Api.Middleware;
using Pudra.Connect.Application.Interfaces;
using Pudra.Connect.Infrastructure.Persistence;
using Pudra.Connect.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Canlı ortamda veya geliştirme dışında, bizim yazdığımız global handler'ı kullan.
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
 
app.UseAuthorization(); 
app.MapControllers();


app.Run();

 