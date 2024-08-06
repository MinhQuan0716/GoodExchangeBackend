using Application.Common;
using WebAPI;
using Infrastructure;
using Infrastructure.Mappers;
using Application.SchemaFilter;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebAPI.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructureService(configuration!.DatabaseConnectionString);
builder.Services.AddWebAPIService(configuration!.JWTSecretKey,configuration!.CacheConnectionString);
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(configuration);
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin() // Use this if you do not need credentials
                              // .SetIsOriginAllowed((host) => true) // Use this if you need to allow specific origins
            .AllowAnyMethod()
            .AllowAnyHeader();
        // .AllowCredentials(); // Only use this if you have a specific need
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

app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSession();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
