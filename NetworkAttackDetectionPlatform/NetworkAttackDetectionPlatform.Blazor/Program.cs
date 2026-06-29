using NetworkAttackDetectionPlatform.Blazor.Components;
using NetworkAttackDetectionPlatform.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register HTTP client for ApiClient (development default - API port 5067)
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5067");
});

// Register HTTP client for DashboardApiService
builder.Services.AddHttpClient<IDashboardApiService, DashboardApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5067");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
