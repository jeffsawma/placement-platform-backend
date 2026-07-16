using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Repositories;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services;
using SystemePlacement.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger pour tester les routes pendant le d�veloppement
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Entrez : Bearer {votre token JWT}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Permet � CurrentUserService de lire l'utilisateur connect� depuis le token JWT
builder.Services.AddHttpContextAccessor();

// Connexion MySQL avec Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("La cha�ne de connexion DefaultConnection est manquante.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Services Dev 1
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStageService, StageService>();


// Autorise le frontend React local a appeler l'API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Services Dev 2 - Entreprises
builder.Services.AddScoped<IEntrepriseService, EntrepriseService>();
// Services Dev 2 - Offres de stage directes
builder.Services.AddScoped<IOffreStageDirecteRepository, OffreStageDirecteRepository>();
builder.Services.AddScoped<IOffreStageDirecteService, OffreStageDirecteService>();

// Services Dev 3 - Offres
builder.Services.AddScoped<IOffreRepository, OffreRepository>();
builder.Services.AddScoped<IOffreService, OffreService>();

// Services Dev 4
builder.Services.AddScoped<ICandidatureRepository, CandidatureRepository>();
builder.Services.AddScoped<ICandidatureService, CandidatureService>();

builder.Services.AddScoped<ISuiviService, SuiviService>();

builder.Services.AddScoped<IDemandeStageRepository, DemandeStageRepository>();
builder.Services.AddScoped<IDemandeStageService, DemandeStageService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();


// Configuration JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("La cl� JWT est manquante dans appsettings.json.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger seulement en d�veloppement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cr�e la base et ajoute les r�les de base si n�cessaire
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(context);
}

app.UseHttpsRedirection();


// Important : CORS avant Authentication/Authorization.
app.UseCors("FrontendPolicy");


// Important : Authentication avant Authorization

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();