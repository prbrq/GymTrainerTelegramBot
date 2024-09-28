using Microsoft.Extensions.Options;
using Telegram.Bot;
using GymTrainerTelegramBot;
using GymTrainerTelegramBot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GymTrainerTelegramBot.Models;
using Microsoft.EntityFrameworkCore;
using GymTrainerTelegramBot.Abstract;

Directory.CreateDirectory("Resources");

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));

        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
                {
                    var botConfiguration = serviceProvider.GetService<IOptions<BotConfiguration>>()?.Value;
                    ArgumentNullException.ThrowIfNull(botConfiguration);
                    var options = new TelegramBotClientOptions(botConfiguration.BotToken);

                    return new TelegramBotClient(options, httpClient);
                });

        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine("Resources", "db")}"));

        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
        services.AddScoped<IChainService, ChainService>();
    })
    .Build();

await host.RunAsync();
