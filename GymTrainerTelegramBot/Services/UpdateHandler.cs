using GymTrainerTelegramBot.Abstract;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace GymTrainerTelegramBot.Services;

public class UpdateHandler(
    ITelegramBotClient bot, 
    ILogger<UpdateHandler> logger, 
    IChainService chainService,
    IScheduleService scheduleService) 
        : IUpdateHandler
{
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            { ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);

        if (msg.Text is not { } messageText)
            return;

        var nextMessageProcessing = chainService.GetNextMessageProcessing(msg.Chat.Id);

        Message sentMessage;

        if (nextMessageProcessing == null)
        {
            sentMessage = await (messageText.Split(' ')[0] switch
            {
                "/inline_buttons" => SendInlineKeyboard(msg),
                "/keyboard" => SendReplyKeyboard(msg),
                "/remove" => RemoveKeyboard(msg),
                "/throw" => FailingHandler(msg),
                "/chain" => TestChain(msg),
                "/schedule" => ShowSchedule(msg),
                _ => Usage(msg)
            });
        }
        else
        {
            sentMessage = await nextMessageProcessing(msg);
        }

        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task<Message> TestChain(Message msg)
    {
        chainService.SetNextMessageProcessing(msg.Chat.Id, TestChain_FirstName);

        return await bot.SendTextMessageAsync(msg.Chat, "Ваше имя?");
    }

    private async Task<Message> TestChain_FirstName(Message msg)
    {
        chainService.SaveChainMessage(msg.Chat.Id, "FirstName", msg.Text);
        chainService.SetNextMessageProcessing(msg.Chat.Id, TestChain_MiddleName);

        return await bot.SendTextMessageAsync(msg.Chat, "Ваше отчество?");
    }

    private async Task<Message> TestChain_MiddleName(Message msg)
    {
        chainService.SaveChainMessage(msg.Chat.Id, "MiddleName", msg.Text);
        chainService.SetNextMessageProcessing(msg.Chat.Id, TestChain_LastName);

        return await bot.SendTextMessageAsync(msg.Chat, "Ваша фамилия?");
    }

    private async Task<Message> TestChain_LastName(Message msg)
    {
        chainService.SaveChainMessage(msg.Chat.Id, "LastName", msg.Text);

        var chainMessages = chainService.LoadChainMessages(msg.Chat.Id);

        var fullName = $"{chainMessages["FirstName"]} {chainMessages["MiddleName"]} {chainMessages["LastName"]}";

        chainService.Clear(msg.Chat.Id);

        return await bot.SendTextMessageAsync(msg.Chat, $"Вау! Вас зовут {fullName}.");
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """
                <b><u>Bot menu</u></b>:
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                /throw          - what happens if handler fails
                /test_db        - test database
            """;
        return await bot.SendTextMessageAsync(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }

    private async Task<Message> ShowSchedule(Message msg)
    {
        await scheduleService.CreateWorkoutsIfNotExistsAsync();

        var availableDates = await scheduleService.GetAvailableDatesAsync();

        var inlineMarkup = new InlineKeyboardMarkup();

        availableDates.ForEach(d => inlineMarkup.AddNewRow($"📅 {d}"));

        return await bot.SendTextMessageAsync(msg.Chat, "Доступные даты:", replyMarkup: inlineMarkup);
    }

    async Task<Message> SendInlineKeyboard(Message msg)
    {
        var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow()
                .AddButton("WithCallbackData", "CallbackData")
                .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        return await bot.SendTextMessageAsync(msg.Chat, "Inline buttons:", replyMarkup: inlineMarkup);
    }

    async Task<Message> SendReplyKeyboard(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow().AddButton("2.1").AddButton("2.2");
        return await bot.SendTextMessageAsync(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
    }

    async Task<Message> RemoveKeyboard(Message msg)
    {
        return await bot.SendTextMessageAsync(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
    }

    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await bot.SendTextMessageAsync(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = [ // displayed result
            new InlineQueryResultArticle("1", "Telegram.Bot", new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2", "is the best", new InputTextMessageContent("world"))
        ];
        await bot.AnswerInlineQueryAsync(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }

    private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
    {
        logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        await bot.SendTextMessageAsync(chosenInlineResult.From.Id, $"You chose result with Id: {chosenInlineResult.ResultId}");
    }

    #endregion

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
