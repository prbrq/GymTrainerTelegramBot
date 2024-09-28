namespace GymTrainerTelegramBot.Abstract;

public interface IChainService
{
    Task SetNextMessageProcessingAsync(long chatId, string method);

    Task<string?> GetNextMessageProcessingAsync(long chatId);

    Task SaveChainMessageAsync(long chatId, string messageKey, string? messageValue);

    Task<Dictionary<string, string?>> LoadChainMessagesAsync(long chatId);

    Task ClearAsync(long chatId);
}
