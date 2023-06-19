using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using EagleRock.Gateway;
using EagleRock.Services;
using EagleRock.Services.Interfaces;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication;
using EagleRock.Services.MessageBroker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Basic")
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options => { });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ITrafficDataService, TrafficDataService>();

var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis:ConnectionString"));
builder.Services.AddSingleton<IDatabase>(provider => redis.GetDatabase());
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IRedisCommand, RedisCommand>();

var rabbitMQClient = new RabbitMQClient(
        builder.Configuration["RabbitMQ:Host"],
        builder.Configuration.GetValue<int>("RabbitMQ:Port"),
        builder.Configuration["RabbitMQ:Username"],
        builder.Configuration["RabbitMQ:Password"]);

builder.Services.AddSingleton<RabbitMQClient>(provider => rabbitMQClient);

var redisSubscriber = new RedisSubscriber(
        rabbitMQClient,
            builder.Configuration["Redis:ConnectionString"],
            builder.Configuration["Redis:ChannelName"]
        );
builder.Services.AddSingleton<RedisSubscriber>(provider => redisSubscriber);

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "basic"
                    }
                },
                new string[] { }
            }
        });
});

builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

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

app.UseAuthentication();
app.UseAuthorization();

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

