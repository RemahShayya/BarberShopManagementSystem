using BarberShopManagementSystem.API.DTO.BarberShopAutoMapper;
using BarberShopManagementSystem.API.Services;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Context;
using BarberShopManagementSystem.Data.Entities;
using BarberShopManagementSystem.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BarberShopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddScoped(typeof(IBarberShopGenericRepo<>), typeof(BarberShopGenericRepo<>));
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddScoped(typeof(BarberShopGenericRepo<>));
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<EmailService>();
// Add this line for AutoMapper
// Register AutoMapper with your specific profile

builder.Services.AddAutoMapper(typeof(BarberShopAutoMapper));
builder.Services.AddOpenApi();


builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = true;
})
        .AddRoles<Role>()
        .AddRoleManager<RoleManager<Role>>()
        .AddEntityFrameworkStores<BarberShopContext>()
        .AddSignInManager<SignInManager<User>>()
        .AddUserManager<UserManager<User>>()
        .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapScalarApiReference(); // Defaults to /scalar

app.Run();