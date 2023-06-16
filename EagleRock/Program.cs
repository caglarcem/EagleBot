using EagleRock.Gateway;
using EagleRock.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITrafficDataService, TrafficDataService>();
builder.Services.AddSingleton<IRedisService, RedisService>();

// Build the configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Register the configuration instance
var configuration = builder.Configuration;
builder.Services.AddSingleton(configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


//INITIAL SETUP - DONE
//API - DONE
//SERVICES - DONE
//REDIS SETUP - DONE
//SWAGGER SETUP - DONE

//WRITE A REDIS SERVICE
// MAKE IT EFFICIENT (SCAN)

//VALIDATION 
//EXCEPTION HANDLING - DONE
// UNIT TESTING

//RABBITMQ SETUP

//PERFORMANCE can it handle thousand requests at a time
//rate limiting / throttling?
//AUTHENTICATION mechanism

// LOAD TESTING
