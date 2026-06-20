using System.Text;
using GatherUp.API.Middleware;
using GatherUp.BL;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Notifications;
using GatherUp.Infrastructure.Repositories;
using GatherUp.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GatherUp API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSingleton<IRepository<Person>>(_ =>
    new XmlRepository<Person>(Path.Combine("..", "XML", "Person.xml"), useSerializer: true));

builder.Services.AddSingleton<IRepository<Event>>(_ =>
    new XmlRepository<Event>(Path.Combine("..", "XML", "Event.xml"), useSerializer: true));

builder.Services.AddScoped<IRepository<VendorAllocation>>(_ =>
    new XmlRepository<VendorAllocation>(Path.Combine("..", "XML", "VendorAllocation.xml"), useSerializer: true));

builder.Services.AddScoped<IRepository<Poll>>(_ =>
    new XmlRepository<Poll>(Path.Combine("..", "XML", "Poll.xml"), useSerializer: true));

builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();

builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

builder.Services.AddSingleton<EventNotifier>();
builder.Services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<EventNotifier>());
builder.Services.AddSingleton<IManagerNotificationEvents>(sp => sp.GetRequiredService<EventNotifier>());
builder.Services.AddSingleton<IParticipantNotificationEvents>(sp => sp.GetRequiredService<EventNotifier>());
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EventDashboardService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<PollService>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("חובה להגדיר פרק Jwt בקובץ appsettings.json.");
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

var smtpSettings = builder.Configuration.GetSection("Smtp").Get<SmtpSettings>()
    ?? throw new InvalidOperationException("חובה להגדיר פרק Smtp בקובץ appsettings.json.");
builder.Services.AddSingleton(smtpSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.Services.GetRequiredService<NotificationService>();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();