using Microsoft.EntityFrameworkCore;

namespace GymTrainerTelegramBot.Models;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Workout> Workouts { get; set; }
}
