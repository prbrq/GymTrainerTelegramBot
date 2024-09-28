using GymTrainerTelegramBot.Abstract;

namespace GymTrainerTelegramBot.Services;

public class ChainService : IChainService
{
    private static readonly Dictionary<long, string> Chains = [];

    public Task<string?> GetNextMessageProcessingAsync(long chatId)
    {
        if (Chains.TryGetValue(chatId, out var nextMethod))
        {
            return Task.FromResult<string?>(nextMethod);
        }

        return Task.FromResult<string?>(null);
    }

    public Task SetNextMessageProcessingAsync(long chatId, string method)
    {
        Chains[chatId] = method;
        
        return Task.CompletedTask;
    }
}