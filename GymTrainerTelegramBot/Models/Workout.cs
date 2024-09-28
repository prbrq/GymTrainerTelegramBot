namespace GymTrainerTelegramBot.Models;

public class Workout
{
    public DateOnly Date { get; set; }

    public int Hour { get; set; }

    public int Minute { get; set; }

    public int? ChatId { get; set; }
}