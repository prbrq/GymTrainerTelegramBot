using GymTrainerTelegramBot.Abstract;
using Microsoft.Extensions.Logging;

namespace GymTrainerTelegramBot.Services;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);