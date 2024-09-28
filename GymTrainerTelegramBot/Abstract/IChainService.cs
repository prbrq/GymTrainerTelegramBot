using Telegram.Bot.Types;

namespace GymTrainerTelegramBot.Abstract;

public interface IChainService
{
    void SetNextMessageProcessing(long chatId, Func<Message, Task<Message>> method);

    Func<Message, Task<Message>>? GetNextMessageProcessing(long chatId);

    void SaveChainMessage(long chatId, string messageKey, string? messageValue);

    Dictionary<string, string?> LoadChainMessages(long chatId);

    void Clear(long chatId);
}
