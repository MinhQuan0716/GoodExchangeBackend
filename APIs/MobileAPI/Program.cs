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
builder.Services.AddHangfire(configuration => configuration
                     .UseSimpleAssemblyNameTypeSerializer()
                     .UseRecommendedSerializerSettings()
                     .UseInMemoryStorage());
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
builder.Services.AddHangfireServer();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed((host) => true)
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
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PerformanceMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSession();

//app.UseRateLimiter();
//Map hangfire
app.MapHangfireDashboard("/dashboard",new DashboardOptions
{
    Authorization = new[] {new HangfireAuthen(configuration.JWTSecretKey)}
});


app.UseHangfireDashboard();


app.MapControllers();
//Map chathub
app.MapHub<ChatHub>("/chatHub");
//Call hangfire
await app.StartAsync();
RecurringJob.AddOrUpdate<ISubcriptionService>(sub => sub.ExtendSubscription(), "0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
RecurringJob.AddOrUpdate<IPostService>(post => post.RemovePostWhenSubscriptionExpire(), "0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
await app.WaitForShutdownAsync();

