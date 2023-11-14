using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VisualizeDo.Models;

namespace VisualizeDo.Context;

public class VisualizeDoContext : DbContext
{
    public DbSet<Card> Cards { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DotNetEnv.Env.Load();
        string connectionString = DotNetEnv.Env.GetString("CONNECTION_STRING");
        optionsBuilder.UseSqlServer(connectionString);
    }
}