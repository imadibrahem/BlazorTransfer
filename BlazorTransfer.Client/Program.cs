using BlazorTransfer.Client.Services;
using BlazorTransfer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<TransferService>();
builder.Services.AddHttpClient<TransferService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:6500/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// Required for Blazor Server
app.UseStaticFiles(); 
app.UseRouting();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
