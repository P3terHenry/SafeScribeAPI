using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SafeScribeApi.Middleware;
using SafeScribeAPI.Data;
using SafeScribeAPI.Services;
using System.Text;
using SafeScribeAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ?? 1. Configura o EF Core com banco em mem�ria
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("SafeScribeDb"));

// ?? 2. Injeta os servi�os
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();

// ?? 3. Configura��o do JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("?? Jwt:Secret n�o configurado!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ?? 4. Autoriza��o baseada em Roles
builder.Services.AddAuthorization();

// ?? 5. Configura Swagger com suporte a Bearer Token
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SafeScribe API",
        Version = "v1",
        Description = "API de gest�o de notas com autentica��o JWT e controle de acesso por roles."
    });

    // ?? Configura��o do bot�o "Authorize" no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insira o token JWT com 'Bearer ' no in�cio. Ex: Bearer eyJhbGciOiJIUzI1...",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // ?? Aqui est� a mudan�a: opera��o para aplicar Bearer somente quando necess�rio
    c.OperationFilter<OptionalAuthOperationFilter>();

    // Configura o Swagger para ler arquivo XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

// ?? Seed autom�tico do usu�rio admin no InMemory
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Users.Any(u => u.Username == "admin"))
    {
        context.Users.Add(new SafeScribeAPI.Models.User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = SafeScribeAPI.Models.Role.Admin
        });
        context.SaveChanges();
        Console.WriteLine("? Usu�rio admin criado automaticamente no InMemory!");
    }
}

// ?? 6. Swagger no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SafeScribe API v1");
        c.DocumentTitle = "SafeScribe API Docs";
    });
}

app.UseHttpsRedirection();

// ?? 7. Ordem correta do pipeline
app.UseAuthentication();
app.UseMiddleware<JwtBlacklistMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();