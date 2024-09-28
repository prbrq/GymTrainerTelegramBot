using GymTrainerTelegramBot.Abstract;

namespace GymTrainerTelegramBot.Services;

public class ChainService : IChainService
{
    private static readonly Dictionary<long, string?> Chains = [];

    private static readonly Dictionary<long, Dictionary<string, string?>> ChainMessages = [];

    public Task<Dictionary<string, string?>> LoadChainMessagesAsync(long chatId)
    {
        return Task.FromResult(ChainMessages[chatId]);
    }

    public Task<string?> GetNextMessageProcessingAsync(long chatId)
    {
        if (Chains.TryGetValue(chatId, out var nextMethod))
        {
            return Task.FromResult<string?>(nextMethod);
        }

        return Task.FromResult<string?>(null);
    }

    public Task SaveChainMessageAsync(long chatId, string messageKey, string? messageValue)
    {
        if (!ChainMessages.TryGetValue(chatId, out var messages))
        {
            messages = [];
            ChainMessages[chatId] = messages;
        }

        messages[messageKey] = messageValue;

        return Task.CompletedTask;
    }

    public Task SetNextMessageProcessingAsync(long chatId, string method)
    {
        Chains[chatId] = method;
        
        return Task.CompletedTask;
    }

    public Task ClearAsync(long chatId)
    {
        ChainMessages[chatId].Clear();
        Chains[chatId] = null;

        return Task.CompletedTask;
    }
}