using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Patient_Service.Data;
using Patient_Service.Interfaces;
using Patient_Service.Middlewares;
using Patient_Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(policies =>
{
    policies.AddPolicy("p-organization-admin", p =>
    {
        p.RequireRole("Organization.Admin");
    });
});

builder.Services.AddCors(); 
builder.Services.AddControllers();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<IPatientService, PatientService>();
builder.Services.AddTransient<IOrganizationService, OrganizationService>();

builder.Services.AddSingleton<INatsService, NatsService>();

builder.Services.AddHostedService<NatsSubscriptionService>();
builder.Services.AddHostedService<HeartBeatService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    
    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddDbContext<DatabaseContext>(options =>
    options
        .UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("PatientContext") ?? string.Empty));

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// if(!app.Environment.IsDevelopment())
// {
    app.UseErrorMiddleware();
// }

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DatabaseContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();