using Monitoring.Basic.Services;
using Monitoring.Basic.Settings;
using Monitoring.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration.GetSection(nameof(AppSettings)));

builder.Services.AddOptions<BingServiceSettings>().Bind(builder.Configuration.GetSection("BingService"));
builder.Services.AddOptions<SqlSettings>().Bind(builder.Configuration.GetSection("Sql"));
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));

builder.Services.AddHealthChecks();
builder.Services.AddHttpClient<INewsService, CleanBingNewsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health").AllowAnonymous();
    endpoints.MapRazorPages();
});
app.Run();