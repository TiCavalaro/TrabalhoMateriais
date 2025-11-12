using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ProjetoXico.Data;
using System.Globalization;
using System.IO;

var builder = WebApplication.CreateBuilder(args);



string ambienteAtivo = builder.Configuration["AmbienteAtivo"] ?? "QA";

string conexao = ambienteAtivo.ToUpper() switch
{
    "PROD" => builder.Configuration.GetConnectionString("BancoProd"),
    _ => builder.Configuration.GetConnectionString("BancoQA")
};

// Registra o DbContext com a conexão correta
builder.Services.AddDbContext<BancoContext>(options =>
    options.UseMySql(conexao, ServerVersion.AutoDetect(conexao)));


builder.Services.AddControllersWithViews();

// Configura autenticação por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
        options.SlidingExpiration = true;
    });



var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "assets")
    ),
    RequestPath = ""
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


var defaultCulture = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.Run();
