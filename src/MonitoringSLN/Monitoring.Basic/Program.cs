using Monitoring.Basic.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration.GetSection(nameof(AppSettings)));
    
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));
builder.Services.AddHealthChecks();

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