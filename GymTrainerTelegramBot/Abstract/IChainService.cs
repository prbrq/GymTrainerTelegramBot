namespace GymTrainerTelegramBot.Abstract;

public interface IChainService
{
    Task SetNextMessageProcessingAsync(long chatId, string method);

    Task<string?> GetNextMessageProcessingAsync(long chatId);
}
