using Microsoft.EntityFrameworkCore;
using TelemetryService.Data;
using TelemetryService.Repositories;
using TelemetryService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключение к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрация репозитория и сервиса
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<ITelemetryBlService, TelemetryBlService>();

// Hosted service для RabbitMQ
builder.Services.AddHostedService<RabbitMQConsumer>();

var app = builder.Build();

// Применение миграций
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();