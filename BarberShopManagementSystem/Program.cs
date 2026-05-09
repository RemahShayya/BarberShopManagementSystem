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
using System.Text.Json;
using System.Text.Json.Serialization;
using NLog;
using NLog.Web;
using Hangfire;
using Hangfire.SqlServer;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Replace default logging providers with NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.UseNLog();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddDbContext<BarberShopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
    builder.Services.AddScoped(typeof(IBarberShopGenericRepo<>), typeof(BarberShopGenericRepo<>));
    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddScoped(typeof(BarberShopGenericRepo<>));
    builder.Services.AddScoped<IServicesService, ServicesService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IBarberScheduleService, BarberScheduleService>();
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IArchivedAppointmentService, ArchivedAppointmentService>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<JWTService>();
    builder.Services.AddScoped<EmailService>();
    builder.Services.AddScoped<AppointmentCleanupService>();
    builder.Services.AddScoped<ScheduleCleanupService>();
    builder.Services.AddScoped<ReviewNotificationService>();
    builder.Services.AddScoped<EmailHelper>();
    builder.Services.AddHostedService<BackgroundServices>();

    builder.Services.AddHangfire(config =>
        config.UseSqlServerStorage(
            builder.Configuration.GetConnectionString("DefaultConnectionString")
    ));

    builder.Services.AddHangfireServer();


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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:Token"])),
                ValidIssuer = builder.Configuration["JwtToken:Issuer"],
                ValidateIssuer = true,
                ValidateAudience = false
            };
        });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
        });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    app.UseAuthentication();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.MapScalarApiReference(); // Defaults to /scalar

    app.UseHangfireDashboard();

    app.Run();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Application stopped because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
