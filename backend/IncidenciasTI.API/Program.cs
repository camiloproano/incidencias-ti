using Microsoft.EntityFrameworkCore;
using IncidenciasTI.API.Data;
using IncidenciasTI.Configurations;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using IncidenciasTI.Services;

// Configurar convención de BSON para camelCase
var pack = new ConventionPack 
{ 
    new CamelCaseElementNameConvention() 
};
ConventionRegistry.Register("camelCase", pack, t => true);

var builder = WebApplication.CreateBuilder(args);

var pgPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        $"Host=localhost;Port=5432;Database=incidencias_ti;Username=postgres;Password={pgPassword}"
    )
);
// Leer configuración de MongoDB y validar
var mongoDbSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();

if (mongoDbSettings == null || string.IsNullOrEmpty(mongoDbSettings.ConnectionString) || string.IsNullOrEmpty(mongoDbSettings.DatabaseName))
{
    throw new InvalidOperationException("La configuración de MongoDB es inválida o no se encuentra.");
}

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));


// Cliente MongoDB
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(mongoDbSettings.ConnectionString));

builder.Services.AddSingleton(s =>
{
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbSettings.DatabaseName);
});

// Registrar LogService
builder.Services.AddScoped<LogService>();

// Registrar SyncService
builder.Services.AddScoped<SyncService>();

// Registrar MongoToSqlSyncService
builder.Services.AddScoped<MongoToSqlSyncService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection solo en producción
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS
app.UseCors("AllowReact");

// Controllers
app.MapControllers();

app.Run();
