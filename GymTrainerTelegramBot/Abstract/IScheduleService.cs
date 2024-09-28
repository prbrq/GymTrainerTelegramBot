namespace GymTrainerTelegramBot.Abstract;

public interface IScheduleService
{
    Task CreateWorkoutsIfNotExistsAsync();

    Task<List<string>> GetAvailableDatesAsync();
}