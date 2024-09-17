using GymTrainerTelegramBot;
using GymTrainerTelegramBot.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var commands = new List<BotCommand>
{
    new() {Command = "/schedule", Description = "Показать расписание"}
};
using var cts = new CancellationTokenSource();
var config = Config.LoadFromFile("config.json");
var bot = new TelegramBotClient(config.Token, cancellationToken: cts.Token);
await bot.SetMyCommandsAsync(commands);
var me = await bot.GetMeAsync();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

await using var context = new ApplicationContext();
await context.Workouts.AddAsync(new Workout { Id = 0, Time = DateTime.Now, CustomerId = 1 });
await context.SaveChangesAsync();

Console.WriteLine($"@{me.Username} is running...");
Console.ReadLine();
await Task.Delay(-1);
return;

async Task OnError(Exception exception, HandleErrorSource source)
{
    await Console.Out.WriteLineAsync(exception.ToString());
}

async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await bot.SendTextMessageAsync(msg.Chat, "Welcome! Pick one direction",
            replyMarkup: new InlineKeyboardMarkup().AddButtons("Left", "Right"));
    }
}

async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query })
    {
        await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}");
        await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
    }
}
