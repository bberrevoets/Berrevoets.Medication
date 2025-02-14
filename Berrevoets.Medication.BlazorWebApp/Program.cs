using Berrevoets.Medication.BlazorWebApp.Components;
using Berrevoets.Medication.BlazorWebApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using ProtectedLocalStore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddProtectedLocalStore(new EncryptionService());

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
//builder.Services.AddScoped<JwtAuthenticationStateProvider>(sp => (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient("UserApi",
    client => { client.BaseAddress = new Uri("https+http://berrevoets-medication-userapi"); });

builder.Services.AddHttpClient("MedicineCatalogApi",
    client => { client.BaseAddress = new Uri("https+http://berrevoets-medication-medicinecatalog"); });

builder.Services.AddHttpClient("MedicineUsesApi",
    client => { client.BaseAddress = new Uri("https+http://berrevoets-medication-medicineuses"); });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();