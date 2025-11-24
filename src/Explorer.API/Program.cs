using Explorer.API.Middleware;
using Explorer.API.Startup;


using Microsoft.Extensions.FileProviders;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StakeholdersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);
const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.RegisterModules();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("touristOrAuthorPolicy", policy =>
        policy.RequireRole("Tourist", "Author")); // dozvoljava obe uloge
});



var app = builder.Build();


//folder za slike za blog
var webRootPath = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var imagesPath = Path.Combine(webRootPath, "blog-images");
Directory.CreateDirectory(imagesPath);


app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}


//file za slike za blog

app.UseStaticFiles();   //serviranje statickih fajlova

app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}


//DODATO ZA FRONTEND
public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services, string policyName)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, builder =>
            {
                builder.WithOrigins("http://localhost:4200") // Angular dev server
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });
    }
}
