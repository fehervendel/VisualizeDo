using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VisualizeDo.Context;
using VisualizeDo.Models;
using VisualizeDo.Repositories;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

void InitializeDb()
{
   using var db = new VisualizeDoContext();
   InitializeToDos();
   
   void InitializeToDos()
   {
       db.Add(new Card
       {
           Title = "Test2 database", Description = "Write 2nd test to see if the database connection is working as expected",
           Priority = "High", Size = "Tiny"
       });
       db.SaveChanges();
   } 
}
//InitializeDb();

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
                //ValidIssuer = jwtSettings["ValidIssuer"],
                //ValidAudience = jwtSettings["ValidAudience"],
                //IssuerSigningKey = new SymmetricSecurityKey(
                //Encoding.UTF8.GetBytes(jwtSettings["IssuerSigningKey"])
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
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<ICardRepository, CardRepository>();
    builder.Services.AddDbContext<VisualizeDoContext>();
}
app.Run();