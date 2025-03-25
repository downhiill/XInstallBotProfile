using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Context;
using Microsoft.OpenApi.Models;
using XInstallBotProfile.Service.Bot;
using XInstallBotProfile.Service.AdminPanelService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string botToken = "7595356206:AAGj07hm_3ll96KCuQJPBR03v53QDf1tGiU";
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext and services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<UserBotService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add BotService with factory method to resolve dependencies
builder.Services.AddSingleton<BotService>(sp =>
{
    var dbContext = sp.GetRequiredService<ApplicationDbContext>();
    return new BotService(botToken, dbContext);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://weekkkk.github.io") // Разрешённый источник
              .AllowAnyMethod()  // Разрешаем любые HTTP-методы (GET, POST и т. д.)
              .AllowAnyHeader() // Разрешаем любые заголовки
              .AllowCredentials();
    });
});

// Swagger setup (if you already have one)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

// Add background task
builder.Services.AddHostedService<BotStartupService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Включите это, если работаете в режиме без HTTPS, для продакшн — используйте HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "yourIssuer",  // Замените на значение, которое использовалось при создании токена
            ValidAudience = "yourAudience",  // Замените на значение, которое использовалось при создании токена
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("s2hG93b0qy32xvwp1PqX0M1aO9lmU4cT"))  // Замените на ваш секретный ключ
        };
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None; // Разрешаем передачу между сайтами
    options.Secure = CookieSecurePolicy.Always; // Требуем HTTPS
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS for all routes
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
