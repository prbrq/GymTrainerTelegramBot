using GymTrainerTelegramBot;
using Telegram.Bot;

var config = Config.LoadFromFile("config.json");

var bot = new TelegramBotClient(config.Token);

var me = await bot.GetMeAsync();

Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");