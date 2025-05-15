using Eventflow.Application.Startup;
using Eventflow.Authentication;
using Eventflow.Configurations;
using Eventflow.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("DummyScheme")
    .AddScheme<AuthenticationSchemeOptions, DummyAuthHandler>("DummyScheme", null);
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(15);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.RegisterServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    await DbSeeder.SeedAdminUserAsync(userRepository);
}

//using (var scope = app.Services.CreateScope())
//{
//    var countryPopulationService = scope.ServiceProvider.GetRequiredService<CountryPopulationService>();
//    await countryPopulationService.PopulateCountriesAsync();
//}

//using (var scope = app.Services.CreateScope())
//{
//    var countryHolidayService = scope.ServiceProvider.GetRequiredService<NationalEventsService>();
//    await countryHolidayService.PopulateNationalHolidaysAsync();
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
