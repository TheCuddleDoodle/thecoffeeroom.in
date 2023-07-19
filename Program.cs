using Coffeeroom.Models.Domain;
using Microsoft.Extensions.Logging;
using Serilog;
using WebMarkupMin.AspNetCore7;
using WebMarkupMin.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".coffeebreak.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day,outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Services.AddWebMarkupMin(options =>
{
    options.AllowMinificationInDevelopmentEnvironment = false;
    options.AllowCompressionInDevelopmentEnvironment = false;
})
.AddHtmlMinification(options =>
{
    options.MinificationSettings.PreserveNewLines = true;
    options.MinificationSettings.MinifyEmbeddedCssCode = true;
    options.MinificationSettings.RemoveHtmlComments = true;
    options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Medium;
    options.MinificationSettings.RemoveHtmlCommentsFromScriptsAndStyles = true;
    options.MinificationSettings.MinifyEmbeddedCssCode = true;
    options.MinificationSettings.MinifyEmbeddedJsCode = true;
})
.AddXmlMinification()
.AddHttpCompression();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseWebMarkupMin();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();
