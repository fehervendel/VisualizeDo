using VisualizeDo.Context;
using VisualizeDo.Models;
using VisualizeDo.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AddServices();


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
void AddServices()
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<ICardRepository, CardRepository>();
}
app.Run();