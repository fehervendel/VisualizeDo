using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VisualizeDo.Context;
using VisualizeDo.Models;
using VisualizeDo.Repositories;
using VisualizeDo.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>();
IConfiguration configuration = configBuilder.Build();
var validIssuer = configuration["JwtSettings:ValidIssuer"];
var validAudience = configuration["JwtSettings:ValidAudience"];
var issuerSigningKey = configuration["JwtSettings:IssuerSigningKey"];
// Add services to the container.
AddServices();
AddAuthentication();
AddIdentity();
ConfigureSwagger();

var app = builder.Build();
Cors();
AddRoles();
AddAdmin();

void Cors()
{
    app.UseCors(builder =>
    {
        builder.AllowAnyOrigin(); 
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

void AddAuthentication()
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(issuerSigningKey)
                ),
            };
        });
}

void AddServices()
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddScoped<ICardRepository, CardRepository>();
    builder.Services.AddDbContext<VisualizeDoContext>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IBoardRepository, BoardRepository>();
    builder.Services.AddScoped<IListRepository, ListRepository>();
}

void AddIdentity()
{
    builder.Services
        .AddIdentityCore<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<VisualizeDoContext>();
}

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo{ Title = "Demo API", Version = "v1"});
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[]{}
            }
        });
    });
}

void AddRoles()
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var tAdmin = CreateAdminRole(roleManager);
    tAdmin.Wait();

    var tUser = CreateUserRole(roleManager);
    tUser.Wait();
}

async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
{
    await roleManager.CreateAsync(new IdentityRole("Admin"));
}

async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
{
    await roleManager.CreateAsync(new IdentityRole("User"));
}

void AddAdmin()
{
    var tAdmin = CreateAdminIfNotExists();
    tAdmin.Wait();
}

async Task CreateAdminIfNotExists()
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var adminInDb = await userManager.FindByEmailAsync("admin@admin.com");
    if (adminInDb == null)
    {
        var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
        var adminCreated = await userManager.CreateAsync(admin, "admin123");

        if (adminCreated.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

public partial class Program
{ }