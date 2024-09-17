using Microsoft.EntityFrameworkCore;

namespace GymTrainerTelegramBot.Models;

public class ApplicationContext : DbContext
{
    public DbSet<Workout> Workouts { get; set; }

    public ApplicationContext()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=db");
    }
}