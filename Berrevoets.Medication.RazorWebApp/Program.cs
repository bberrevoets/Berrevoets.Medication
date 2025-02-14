using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("UserApi",
    client => { client.BaseAddress = new Uri("https+http://berrevoets-medication-userapi"); });

builder.Services.AddHttpClient("MedicineCatalogApi",
    client => { client.BaseAddress = new Uri("https+http://berrevoets-medication-medicinecatalog"); });

builder.Services.AddHttpClient("MedicineUsesApi",
    client => { client.BaseAddress = new Uri("https+http://berrevoets-medication-medicineuses"); });

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();