using InventarioHerramienta;
using InventarioHerramienta.Interfaces;
using InventarioViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.DataContext;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
ConfigurationManager configuration = builder.Configuration;

var cnn = configuration.GetConnectionString("cnn");
builder.Services.AddDbContext<InventarioDbContext>(options => options.UseSqlServer(cnn));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.Cookie.Name = "inventariotdscookie";
    });
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
});


var ms = configuration.GetSection("MailSettings");

builder.Services.Configure<MailSettings>(ms);
builder.Services.AddTransient<IMailService, MailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(options => options.AllowAnyOrigin());

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
