using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configurar Entity Framework con MySQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(configuration.GetConnectionString("MySql"))
    ));




builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllersWithViews();

// Registrar repositorios
builder.Services.AddScoped<IRepositorioCliente, RepositorioCliente>();
builder.Services.AddScoped<IRepositorioProductos, RepositorioProductos>();
builder.Services.AddScoped<IRepositorioRecetaProducto, RepositorioRecetaProducto>();
builder.Services.AddScoped<IRepositorioReceta_Medica, RepositorioReceta_Medica>();
builder.Services.AddScoped<IRepositorioVentas, RepositorioVentas>();
builder.Services.AddScoped<IRepositorioDetalleVentas, RepositorioDetalleVentas>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>//el sitio web valida con cookie
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(5);//Tiempo de expiración
    })
    .AddJwtBearer(options =>//la api web valida con token
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["TokenAuthentication:Issuer"],
            ValidAudience = configuration["TokenAuthentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
                configuration["TokenAuthentication:SecretKey"])),
        };
    });



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});
builder.Services.AddMvc();


var app = builder.Build();



app.UseCors();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
