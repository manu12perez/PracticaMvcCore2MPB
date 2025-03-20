using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using PracticaMvcCore2MPB.Data;
using PracticaMvcCore2MPB.Repositories;

var builder = WebApplication.CreateBuilder(args);

/*****************************************************************************************************************************************/
// Add services to the container.
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();

//Las politicas se agregan a Authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//});

// Configuración de SQL Server
string connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDbContext<LibrosContext>(options => options.UseSqlServer(connectionString));

// Registro de repositorios
builder.Services.AddScoped<RepositoryLibros>();

//Añadimos Session y memoria distribuida
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

//Añadimos authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(
    CookieAuthenticationDefaults.AuthenticationScheme,
    config =>
    {
        config.AccessDeniedPath = "/Managed/ErrorAcceso";
    });
/*****************************************************************************************************************************************/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

/*****************************************************************************************************************************************/
//app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();
app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});
/*****************************************************************************************************************************************/

app.Run();
