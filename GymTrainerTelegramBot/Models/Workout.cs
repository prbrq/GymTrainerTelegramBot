namespace GymTrainerTelegramBot.Models;

public class Workout
{
    public int Id { get; set; }

    public DateTime Time { get; set; }

    public int? CustomerId { get; set; }
}