using Microsoft.EntityFrameworkCore;
using Portifolio.Models.Data;
using Portifolio.Services.Authentication;
using DotNetEnv;
using Portifolio.Configurations;
using Portifolio.Services.Email;
using Portifolio.Services.Register;
using Microsoft.AspNetCore.Authentication.Cookies;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use ConnectionString 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Check null ConnectionString
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada.");
}

// Use SQLServer 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IRegisterService, RegisterService>();

builder.Services.Configure<SmtpSettings>(options =>
{
    options.Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "";

    // Converte a porta para int de forma segura, usando 587 como padrão caso falhe
    options.Port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587;

    options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
    options.Encryption = Environment.GetEnvironmentVariable("SMTP_ENCRYPTION") ?? "";
    options.FromAddress = Environment.GetEnvironmentVariable("SMTP_FROM_ADDRESS") ?? "";
    options.FromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
