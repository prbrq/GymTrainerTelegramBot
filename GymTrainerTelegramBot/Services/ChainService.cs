using GymTrainerTelegramBot.Abstract;
using Telegram.Bot.Types;

namespace GymTrainerTelegramBot.Services;

public class ChainService : IChainService
{
    private static readonly Dictionary<long, Func<Message, Task<Message>>?> Chains = [];

    private static readonly Dictionary<long, Dictionary<string, string?>> ChainMessages = [];

    public Dictionary<string, string?> LoadChainMessages(long chatId)
    {
        return ChainMessages[chatId];
    }

    public Func<Message, Task<Message>>? GetNextMessageProcessing(long chatId)
    {
        if (Chains.TryGetValue(chatId, out var nextMethod))
        {
            return nextMethod;
        }

        return null;
    }

    public void SaveChainMessage(long chatId, string messageKey, string? messageValue)
    {
        if (!ChainMessages.TryGetValue(chatId, out var messages))
        {
            messages = [];
            ChainMessages[chatId] = messages;
        }

        messages[messageKey] = messageValue;
    }

    public void SetNextMessageProcessing(long chatId, Func<Message, Task<Message>> method)
    {
        Chains[chatId] = method;
    }

    public void Clear(long chatId)
    {
        ChainMessages[chatId].Clear();
        Chains[chatId] = null;
    }
}