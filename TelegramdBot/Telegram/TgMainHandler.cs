using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramBot.Telegram.Handlers;

namespace TelegramBot.Telegram;

public class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ILogger<TelegramUpdateHandler> _logger;
    private readonly TgHandlerFactory _tgHandlerFactory;

    public TelegramUpdateHandler(ILogger<TelegramUpdateHandler> logger, TgHandlerFactory tgHandlerFactory)
    {
        _logger = logger;
        _tgHandlerFactory = tgHandlerFactory;
    }
    
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            var handler = _tgHandlerFactory.GetHandler(update.Type);
            return handler.HandleAsync(update, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return Task.CompletedTask;
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError($"При выполнении команды произошла ошибка: {exception}");
        return Task.CompletedTask;
    }
}