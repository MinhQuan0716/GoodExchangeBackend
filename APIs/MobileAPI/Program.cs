using Application.Common;
using Application.InterfaceService;
using Application.SchemaFilter;
using Application.Service;
using Application.VnPay.Config;
using Hangfire;
using Infrastructure;
using Infrastructure.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MobileAPI;
using MobileAPI.Hubs;
using MobileAPI.Middleware;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(configuration!.DatabaseConnectionString);
builder.Services.AddMobileAPIService(configuration!.JWTSecretKey,configuration!.CacheConnectionString);
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddSingleton(configuration);
builder.Services.Configure<VnPayConfig>(builder.Configuration.GetSection(VnPayConfig.ConfigName));
/*builder.Services.AddHangfire(configuration => configuration
                     .UseSimpleAssemblyNameTypeSerializer()
                     .UseRecommendedSerializerSettings()
                     .UseInMemoryStorage());*/
builder.Services.AddSwaggerGen(opt =>
{
opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    In = ParameterLocation.Header,
    Description = "Please enter token",
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "bearer"
});
opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
     });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
    opt.SchemaFilter<RegisterSchemaFilter>();
});
//builder.Services.AddHangfireServer();

/*builder.Services.AddSingleton<ISocketServerService>(new SocketServerService(1234));*/
/*builder.Services.AddHostedService<SocketServerBackgroundService>();*/
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API");

    });
    app.ApplyMigration();
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API");

    });
}
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseMiddleware<PerformanceMiddleware>();
app.UseSession();
/*app.UseWebSockets();*/

//app.UseRateLimiter();
//app.MapHangfireDashboard("/dashboard");

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

/*var socketServer = app.Services.GetRequiredService<ISocketServerService>();
socketServer.Start();*/

app.Run();
