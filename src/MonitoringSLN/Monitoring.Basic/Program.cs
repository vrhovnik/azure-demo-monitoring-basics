var builder = WebApplication.CreateBuilder(args);

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