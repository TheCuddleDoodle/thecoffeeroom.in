using Coffeeroom.Repositories;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using WebMarkupMin.AspNetCore7;
using WebMarkupMin.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("https://laymaann.in");
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
});

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/coffeelog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".coffeebreak.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
});

builder.Services.AddScoped<IUserProfileRepo, UserProfileRepo>();
builder.Services.AddScoped<IMailingListRepo, MailingListRepo>();
builder.Services.AddScoped<IPassResetTokenRepo,PassResetTokenRepo>();

builder.Services.AddWebMarkupMin(options =>
{
    options.AllowMinificationInDevelopmentEnvironment = false;
    options.AllowCompressionInDevelopmentEnvironment = false;
})
.AddHtmlMinification(options =>
{
    options.MinificationSettings.PreserveNewLines = false;
    options.MinificationSettings.MinifyEmbeddedCssCode = true;
    options.MinificationSettings.RemoveHtmlComments = true;
    options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
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
app.UseCors();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();
