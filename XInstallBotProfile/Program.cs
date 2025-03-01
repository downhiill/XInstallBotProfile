using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile;

var builder = WebApplication.CreateBuilder(args);

string botToken = "7947836624:AAHGUWdQslw4iCpQJDIG_oURFdTfvGOZje8";
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext and services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<UserService>();

// Add BotService with factory method to resolve dependencies
builder.Services.AddSingleton<BotService>(sp =>
{
    var dbContext = sp.GetRequiredService<ApplicationDbContext>();
    return new BotService(botToken, dbContext);
});

// Add background task
builder.Services.AddHostedService<BotStartupService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
